using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class User
{
    [Key]
    public string Email { get; set; } = string.Empty;
    public byte[]? Password { get; set; }
    public byte[]? HashKey { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public Client? Client { get; set; }
    public Freelancer? Freelancer { get; set; }
}