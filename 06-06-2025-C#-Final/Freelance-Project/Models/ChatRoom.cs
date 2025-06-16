using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class ChatRoom
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    public Guid ClientId { get; set; }
    public Guid FreelancerId { get; set; }
    public Guid ProjectId { get; set; }

    public Client? Client { get; set; }
    public Freelancer? Freelancer { get; set; }
    
    public ICollection<ChatMessage>? Messages { get; set; }
}