using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateChatMessageDTO
{
    [Required(ErrorMessage = "Content is required.")]
    public string Content { get; set; }
    [Required(ErrorMessage = "Chat Room ID is required.")]
    public Guid ChatRoomId { get; set; }
    [Required(ErrorMessage = "Sender ID is required.")]
    public Guid SenderId { get; set; }
}
public class UpdateChatMessageDTO
{
    [Required(ErrorMessage = "Content is required.")]
    public string? Content { get; set; }
}
public class ReadChatMessageDTO
{
    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
}
public class ChatMessageResponseDTO
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsRead { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Guid ChatRoomId { get; set; }
    public Guid SenderId { get; set; }
}