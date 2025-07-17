using EventPlannerAPI.DTOs;
using EventPlannerAPI.Models;

namespace EventPlannerAPI.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    string GenerateJwtToken(User user);
}

public interface IEventService
{
    Task<EventDto?> CreateEventAsync(CreateEventDto createEventDto, int creatorId);
    Task<List<EventDto>> GetEventsAsync(int? userId = null);
    Task<EventDto?> GetEventByIdAsync(int eventId, int? userId = null);
    Task<EventDto?> UpdateEventAsync(int eventId, UpdateEventDto updateEventDto, int userId);
    Task<bool> DeleteEventAsync(int eventId, int userId);
}

public interface IParticipantService
{
    Task<bool> InviteParticipantAsync(int eventId, InviteDto inviteDto, int userId);
    Task<bool> JoinEventAsync(int eventId, int userId, string? inviteEmail = null);
    Task<bool> UpdateRsvpAsync(int eventId, RsvpDto rsvpDto, int userId);
    Task<List<ParticipantDto>> GetEventParticipantsAsync(int eventId, int userId);
}
