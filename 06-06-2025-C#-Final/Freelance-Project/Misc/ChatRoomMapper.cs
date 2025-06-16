using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Mappers;

public static class ChatRoomMapper
{
    public static ChatRoomResponseDTO ToResponseDTO(ChatRoom chatRoom)
    {
        return new ChatRoomResponseDTO
        {
            Id = chatRoom.Id,
            Name = chatRoom.Name,
            CreatedAt = chatRoom.CreatedAt,
            UpdatedAt = chatRoom.UpdatedAt,
            DeletedAt = chatRoom.DeletedAt,
            IsActive = chatRoom.IsActive,
            ClientId = chatRoom.ClientId,
            FreelancerId = chatRoom.FreelancerId,
            ProjectId = chatRoom.ProjectId,
            ClientName = chatRoom.Client?.Username,
            FreelancerName = chatRoom.Freelancer?.Username,
            Messages = chatRoom.Messages?
                .Where(m => m.IsActive)
                .OrderBy(m => m.SentAt)
                .Select(ChatMessageMapper.ToResponseDTO)
                .ToList()
        };
    }

    public static ChatRoom CreateFromDTO(CreateChatRoomDTO dto)
    {
        return new ChatRoom
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ClientId = dto.ClientId,
            FreelancerId = dto.FreelancerId,
            ProjectId = dto.ProjectId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }
}