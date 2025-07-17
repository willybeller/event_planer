using System.ComponentModel.DataAnnotations;

namespace EventPlannerAPI.DTOs;

public class CreateEventDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly Time { get; set; }
    
    public bool IsPublic { get; set; } = true;
}

public class UpdateEventDto
{
    [MaxLength(255)]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public DateOnly? Date { get; set; }
    
    public TimeOnly? Time { get; set; }
    
    public bool? IsPublic { get; set; }
}

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }
    public bool IsPublic { get; set; }
    public int CreatorId { get; set; }
    public UserDto Creator { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    
    // Informations spécifiques à l'utilisateur connecté
    public bool IsCurrentUserAdmin { get; set; }
    public string? CurrentUserRsvpStatus { get; set; }
}
