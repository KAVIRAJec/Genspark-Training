using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Mappers;

public static class ChatMessageMapper
{
    public static ChatMessageResponseDTO ToResponseDTO(ChatMessage message)
    {
        return new ChatMessageResponseDTO
        {
            Id = message.Id,
            Content = message.Content,
            SentAt = message.SentAt,
            UpdatedAt = message.UpdatedAt,
            DeletedAt = message.DeletedAt,
            IsRead = message.IsRead,
            IsActive = message.IsActive,
            ChatRoomId = message.ChatRoomId,
            SenderId = message.SenderId
        };
    }

    public static ChatMessage CreateFromDTO(CreateChatMessageDTO dto)
    {
        return new ChatMessage
        {
            Id = Guid.NewGuid(),
            Content = dto.Content,
            ChatRoomId = dto.ChatRoomId,
            SenderId = dto.SenderId,
            SentAt = DateTime.UtcNow,
            IsRead = false,
            IsActive = true
        };
    }

    // public static void UpdateFromDTO(ChatMessage message, UpdateChatMessageDTO dto)
    // {
    //     message.Content = dto.Content;
    //     message.UpdatedAt = DateTime.UtcNow;
    // }
}