using System.ComponentModel.DataAnnotations;

namespace EventPlannerAPI.Models;

public class Event
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public DateOnly Date { get; set; }
    
    [Required]
    public TimeOnly Time { get; set; }
    
    // Garde l'ancienne propriété pour compatibilité avec la DB existante
    public bool IsPublic { get; set; } = true;
    
    public bool IsPrivate { get; set; } = false;
    
    public string InvitedEmails { get; set; } = string.Empty; // JSON array of emails
    
    [Required]
    public int CreatorId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public User Creator { get; set; } = null!;
    public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
}
