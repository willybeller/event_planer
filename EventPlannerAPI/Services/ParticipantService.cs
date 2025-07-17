using EventPlannerAPI.Data;
using EventPlannerAPI.DTOs;
using EventPlannerAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerAPI.Services;

public class ParticipantService : IParticipantService
{
    private readonly ApplicationDbContext _context;

    public ParticipantService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> InviteParticipantAsync(int eventId, InviteDto inviteDto, int userId)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null) return false;

        // Vérifier que l'utilisateur est admin de l'événement
        var userParticipation = eventEntity.Participants.FirstOrDefault(p => p.UserId == userId);
        if (userParticipation?.IsAdmin != true) return false;

        // Vérifier si l'email est déjà invité
        if (eventEntity.Participants.Any(p => p.Email == inviteDto.Email))
        {
            return false; // Déjà invité
        }

        // Vérifier si l'email correspond à un utilisateur existant
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == inviteDto.Email);

        var participant = new EventParticipant
        {
            EventId = eventId,
            Email = inviteDto.Email,
            UserId = existingUser?.Id,
            RsvpStatus = "pending",
            IsAdmin = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.EventParticipants.Add(participant);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> JoinEventAsync(int eventId, int userId, string? inviteEmail = null)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Participants)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null) return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // Vérifier si l'utilisateur peut rejoindre l'événement
        var canJoin = eventEntity.IsPublic || 
                     eventEntity.CreatorId == userId ||
                     (inviteEmail != null && eventEntity.Participants.Any(p => p.Email == inviteEmail));

        if (!canJoin) return false;

        // Vérifier si l'utilisateur est déjà participant
        var existingParticipation = eventEntity.Participants.FirstOrDefault(p => p.UserId == userId);
        if (existingParticipation != null) return false;

        EventParticipant participant;

        // Si l'utilisateur rejoint via une invitation par email
        if (inviteEmail != null)
        {
            var invitation = eventEntity.Participants.FirstOrDefault(p => p.Email == inviteEmail && p.UserId == null);
            if (invitation != null)
            {
                // Mettre à jour l'invitation existante
                invitation.UserId = userId;
                invitation.RsvpStatus = "yes";
            }
            else
            {
                return false; // Invitation non trouvée
            }
        }
        else
        {
            // Créer une nouvelle participation pour un événement public
            participant = new EventParticipant
            {
                EventId = eventId,
                Email = user.Email,
                UserId = userId,
                RsvpStatus = "yes",
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.EventParticipants.Add(participant);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateRsvpAsync(int eventId, RsvpDto rsvpDto, int userId)
    {
        var participation = await _context.EventParticipants
            .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId);

        if (participation == null) return false;

        participation.RsvpStatus = rsvpDto.Status;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<ParticipantDto>> GetEventParticipantsAsync(int eventId, int userId)
    {
        var eventEntity = await _context.Events
            .Include(e => e.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventEntity == null) return new List<ParticipantDto>();

        // Vérifier que l'utilisateur a accès à l'événement
        var hasAccess = eventEntity.IsPublic || 
                       eventEntity.CreatorId == userId || 
                       eventEntity.Participants.Any(p => p.UserId == userId);

        if (!hasAccess) return new List<ParticipantDto>();

        return eventEntity.Participants.Select(p => new ParticipantDto
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
        }).ToList();
    }
}
