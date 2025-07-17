using EventPlannerAPI.Data;
using EventPlannerAPI.DTOs;
using EventPlannerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerAPI.Services;

public class EventService : IEventService
{
    private readonly ApplicationDbContext _context;

    public EventService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, int creatorId)
    {
        var eventEntity = new Event
        {
            Title = createEventDto.Title,
            Description = createEventDto.Description,
            Date = createEventDto.Date,
            Time = createEventDto.Time,
            IsPublic = createEventDto.IsPublic,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventEntity);
        await _context.SaveChangesAsync();

        // Ajouter le créateur comme participant admin
        var creatorParticipant = new EventParticipant
        {
            EventId = eventEntity.Id,
            Email = (await _context.Users.FindAsync(creatorId))!.Email,
            UserId = creatorId,
            RsvpStatus = "yes",
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.EventParticipants.Add(creatorParticipant);
        await _context.SaveChangesAsync();

        return await GetEventByIdAsync(eventEntity.Id, creatorId);
    }

    public async Task<List<EventDto>> GetEventsAsync(int? userId = null)
    {
        var query = _context.Events
            .Include(e => e.Creator)
            .Include(e => e.Participants)
                .ThenInclude(p => p.User)
            .AsQueryable();

        if (userId.HasValue)
        {
            // Retourner les événements publics + ceux où l'utilisateur est invité + ceux créés par l'utilisateur
            query = query.Where(e => 
                e.IsPublic || 
                e.CreatorId == userId || 
                e.Participants.Any(p => p.UserId == userId));
        }
        else
        {
            // Retourner seulement les événements publics si pas connecté
            query = query.Where(e => e.IsPublic);
        }

        var events = await query.OrderBy(e => e.Date).ThenBy(e => e.Time).ToListAsync();

        return events.Select(e => MapToEventDto(e, userId)).ToList();
    }

    public async Task<EventDto?> GetEventByIdAsync(int eventId, int? userId = null)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Creator)
            .Include(e => e.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null) return null;

        // Vérifier les permissions d'accès
        if (!eventEntity.IsPublic && userId.HasValue)
        {
            var hasAccess = eventEntity.CreatorId == userId || 
                           eventEntity.Participants.Any(p => p.UserId == userId);
            if (!hasAccess) return null;
        }
        else if (!eventEntity.IsPublic && !userId.HasValue)
        {
            return null; // Événement privé et utilisateur non connecté
        }

        return MapToEventDto(eventEntity, userId);
    }

    public async Task<EventDto?> UpdateEventAsync(int eventId, UpdateEventDto updateEventDto, int userId)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null || eventEntity.CreatorId != userId)
        {
            return null; // Événement non trouvé ou pas d'autorisation
        }

        // Mettre à jour les propriétés non nulles
        if (!string.IsNullOrEmpty(updateEventDto.Title))
            eventEntity.Title = updateEventDto.Title;
        
        if (!string.IsNullOrEmpty(updateEventDto.Description))
            eventEntity.Description = updateEventDto.Description;
        
        if (updateEventDto.Date.HasValue)
            eventEntity.Date = updateEventDto.Date.Value;
        
        if (updateEventDto.Time.HasValue)
            eventEntity.Time = updateEventDto.Time.Value;
        
        if (updateEventDto.IsPublic.HasValue)
            eventEntity.IsPublic = updateEventDto.IsPublic.Value;

        await _context.SaveChangesAsync();
        
        return await GetEventByIdAsync(eventId, userId);
    }

    public async Task<bool> DeleteEventAsync(int eventId, int userId)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity == null || eventEntity.CreatorId != userId)
        {
            return false; // Événement non trouvé ou pas d'autorisation
        }

        _context.Events.Remove(eventEntity);
        await _context.SaveChangesAsync();
        return true;
    }

    private EventDto MapToEventDto(Event eventEntity, int? userId)
    {
        var userParticipation = userId.HasValue ? 
            eventEntity.Participants.FirstOrDefault(p => p.UserId == userId) : null;

        return new EventDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Time = eventEntity.Time,
            IsPublic = eventEntity.IsPublic,
            CreatorId = eventEntity.CreatorId,
            Creator = new UserDto
            {
                Id = eventEntity.Creator.Id,
                Name = eventEntity.Creator.Name,
                Email = eventEntity.Creator.Email,
                CreatedAt = eventEntity.Creator.CreatedAt
            },
            CreatedAt = eventEntity.CreatedAt,
            Participants = eventEntity.Participants.Select(p => new ParticipantDto
            {
                Id = p.Id,
                Email = p.Email,
                User = p.User != null ? new UserDto
                {
                    Id = p.User.Id,
                    Name = p.User.Name,
                    Email = p.User.Email,
                    CreatedAt = p.User.CreatedAt
                } : null,
                RsvpStatus = p.RsvpStatus,
                IsAdmin = p.IsAdmin,
                CreatedAt = p.CreatedAt
            }).ToList(),
            IsCurrentUserAdmin = userParticipation?.IsAdmin ?? false,
            CurrentUserRsvpStatus = userParticipation?.RsvpStatus
        };
    }
}
