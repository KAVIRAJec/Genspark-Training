using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IChatService
{
    public Task<ChatMessageResponseDTO> SendMessage(string content, Guid senderId, Guid chatRoomId);
    public Task<ChatMessageResponseDTO> UpdateMessage(Guid messageId, UpdateChatMessageDTO updateChatMessageDTO);
    public Task<ChatMessageResponseDTO> SetMessageRead(Guid messageId, ReadChatMessageDTO updateChatMessageDTO);
    public Task<ChatMessageResponseDTO> DeleteMessage(Guid messageId, Guid chatRoomId);

    public Task<PagedResponse<ChatMessageResponseDTO>> GetMessagesByChatRoomId(Guid chatRoomId, PaginationParams paginationParams);
    public Task<ChatMessageResponseDTO> GetMessageById(Guid messageId);
    public Task<ChatRoomResponseDTO> GetChatById(Guid chatRoomId);

    public Task<ChatRoomResponseDTO> GetChatRoomByProjectId(Guid projectId);
    public Task<PagedResponse<ChatRoomResponseDTO>> GetChatRoomByUserId(Guid userId, PaginationParams paginationParams);
}