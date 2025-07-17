using System.ComponentModel.DataAnnotations;

namespace EventPlannerAPI.Models;

public class EventParticipant
{
    public int Id { get; set; }
    
    [Required]
    public int EventId { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    public int? UserId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string RsvpStatus { get; set; } = "pending"; // "yes", "no", "maybe", "pending"
    
    public bool IsAdmin { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Event Event { get; set; } = null!;
    public User? User { get; set; }
}
