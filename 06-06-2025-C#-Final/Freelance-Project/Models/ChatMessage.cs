using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class ChatMessage
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsRead { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }

    public ChatRoom? ChatRoom { get; set; }
}