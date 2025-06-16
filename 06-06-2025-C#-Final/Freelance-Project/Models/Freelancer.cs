using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class Freelancer
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? ProfileUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User User { get; set; }
    public ICollection<Skill>? Skills { get; set; }
    public ICollection<Proposal>? Proposals { get; set; }
    public ICollection<ChatRoom>? ChatRooms { get; set; }

}