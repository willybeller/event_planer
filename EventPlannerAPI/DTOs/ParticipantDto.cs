using System.ComponentModel.DataAnnotations;

namespace EventPlannerAPI.DTOs;

public class InviteDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

public class RsvpDto
{
    [Required]
    [RegularExpression("^(yes|no|maybe)$", ErrorMessage = "Status must be 'yes', 'no', or 'maybe'")]
    public string Status { get; set; } = string.Empty;
}

public class ParticipantDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string RsvpStatus { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
}
