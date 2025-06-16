    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Freelance_Project.Models;

public class Client
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? ProfileUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public User User { get; set; }
    public ICollection<Project>? Projects { get; set; }
    public ICollection<ChatRoom>? ChatRooms { get; set; }
}