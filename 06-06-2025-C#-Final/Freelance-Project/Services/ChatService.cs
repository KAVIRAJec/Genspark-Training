using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Mappers;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class ChatService : IChatService
{
    private readonly IRepository<Guid, ChatMessage> _chatMessageRepository;
    private readonly IRepository<Guid, ChatRoom> _chatRoomRepository;
    private readonly FreelanceDBContext _appContext;

    public ChatService(IRepository<Guid, ChatMessage> chatMessageRepository,
                       IRepository<Guid, ChatRoom> chatRoomRepository,
                       FreelanceDBContext appContext)
    {
        _chatMessageRepository = chatMessageRepository;
        _chatRoomRepository = chatRoomRepository;
        _appContext = appContext;
    }

    public async Task<ChatMessageResponseDTO> SendMessage(string content, Guid senderId, Guid chatRoomId)
    {
        var message = ChatMessageMapper.CreateFromDTO(new CreateChatMessageDTO
        {
            Content = content,
            SenderId = senderId,
            ChatRoomId = chatRoomId
        });

        var createdMessage = await _chatMessageRepository.Add(message);
        if (createdMessage == null) throw new AppException("Failed to create chat message.", 500);
        return ChatMessageMapper.ToResponseDTO(createdMessage);
    }

    public async Task<ChatMessageResponseDTO> UpdateMessage(Guid messageId, UpdateChatMessageDTO updateChatMessageDTO)
    {
        var message = await _chatMessageRepository.Get(messageId);
        if (message == null) throw new AppException("Message not found.", 404);
        if (message.IsActive == false) throw new AppException("Cannot update a deleted message.", 400);

        if (string.IsNullOrEmpty(updateChatMessageDTO.Content))
            throw new AppException("Content cannot be empty.", 400);

        if (_appContext.Clients.Any(c => c.Id == message.SenderId) == false && _appContext.Freelancers.Any(p => p.Id == message.SenderId) == false)
            throw new AppException("Sender does not exist.", 404);

        message.Content = updateChatMessageDTO.Content;
        message.UpdatedAt = DateTime.UtcNow;

        var updatedMessage = await _chatMessageRepository.Update(message.Id, message);
        if (updatedMessage == null) throw new AppException("Failed to update chat message.", 500);
        return ChatMessageMapper.ToResponseDTO(updatedMessage);
    }

    public async Task<ChatMessageResponseDTO> SetMessageRead(Guid messageId, ReadChatMessageDTO updateChatMessageDTO)
    {
        var message = await _chatMessageRepository.Get(messageId);
        if (message == null) throw new AppException("Message not found.", 404);
        if (message.ChatRoomId != updateChatMessageDTO.ChatRoomId) throw new AppException("Message does not belong to the specified chat room.", 400);
        if(message.IsActive == false) throw new AppException("Cannot update a deleted message.", 400);
        if (message.IsRead) throw new AppException("Message is already marked as read.", 400);

        // Mark all previous messages in the chat room as read for this user
        var messagesToUpdate = _appContext.ChatMessages
            .Where(m => m.ChatRoomId == message.ChatRoomId
                && m.SentAt <= message.SentAt 
                && m.SenderId != updateChatMessageDTO.SenderId
                && !m.IsRead 
                && m.IsActive)
            .ToList();

        foreach (var msg in messagesToUpdate)
        {
            msg.IsRead = true;
        }

        await _appContext.SaveChangesAsync();

        // Refresh the message after update
        var updatedMessage = await _chatMessageRepository.Get(message.Id);
        if (updatedMessage == null) throw new AppException("Failed to update chat message.", 500);
        return ChatMessageMapper.ToResponseDTO(updatedMessage);
    }

    public async Task<ChatMessageResponseDTO> DeleteMessage(Guid messageId, Guid chatRoomId)
    {
        var message = await _chatMessageRepository.Get(messageId);
        if (message == null || message.ChatRoomId != chatRoomId) throw new AppException("Message not found or does not belong to the specified chat room.", 404);
        if (message.IsActive == false) throw new AppException("Message already deleted.", 400);

        message.DeletedAt = DateTime.UtcNow;
        message.IsActive = false;

        var deletedMessage = await _chatMessageRepository.Update(message.Id, message);
        if (deletedMessage == null) throw new AppException("Failed to delete chat message.", 500);
        return ChatMessageMapper.ToResponseDTO(deletedMessage);
    }

    public async Task<PagedResponse<ChatMessageResponseDTO>> GetMessagesByChatRoomId(Guid chatRoomId, PaginationParams paginationParams)
    {
        var query = _appContext.ChatMessages
            .Where(m => m.ChatRoomId == chatRoomId && m.IsActive)
            .Include(cm => cm.ChatRoom)
            .OrderByDescending(m => m.SentAt)
            .Select(m => ChatMessageMapper.ToResponseDTO(m));

        return await query.ToPagedResponse(paginationParams);
    }

    public async Task<ChatMessageResponseDTO> GetMessageById(Guid messageId)
    {
        if (messageId == Guid.Empty) throw new AppException("Message ID is required.", 400);

        var message = await _chatMessageRepository.Get(messageId);
        if (message == null || !message.IsActive) throw new AppException("Message not found or has been deleted.", 404);

        return ChatMessageMapper.ToResponseDTO(message);
    }
    public async Task<ChatRoomResponseDTO> GetChatById(Guid chatRoomId)
    {
        if (chatRoomId == Guid.Empty) throw new AppException("Chat room ID is required.", 400);

        var chatRoom = await _chatRoomRepository.Get(chatRoomId);
        if (chatRoom == null || !chatRoom.IsActive) throw new AppException("Chat room not found or has been deleted.", 404);

        return ChatRoomMapper.ToResponseDTO(chatRoom);
    }

    public async Task<ChatRoomResponseDTO> GetChatRoomByProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty) throw new AppException("Project ID is required.", 400);

        var projectExists = await _appContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId && p.IsActive);
        if (projectExists == null) throw new AppException("Project not found.", 404);
        if (projectExists.Status != "In Progress")
            throw new AppException("Chat room are only available for projects that are in progress.", 403);

        var chatRoom = await _appContext.ChatRooms
            .FirstOrDefaultAsync(cr => cr.ProjectId == projectId && cr.IsActive);

        if (chatRoom == null) throw new AppException("Chat room not found for the specified project.", 404);

        return ChatRoomMapper.ToResponseDTO(chatRoom);
    }

    public async Task<PagedResponse<ChatRoomResponseDTO>> GetChatRoomByUserId(Guid userId, PaginationParams paginationParams)
    {
        if (userId == Guid.Empty) throw new AppException("User ID is required.", 400);

        var userExists = _appContext.Clients.Any(c => c.Id == userId) || _appContext.Freelancers.Any(f => f.Id == userId);
        if (!userExists) throw new AppException("User not found.", 404);

        var query = _appContext.ChatRooms
            .Where(cr => (cr.ClientId == userId || cr.FreelancerId == userId) && cr.IsActive)
            .Include(cr => cr.Client)
            .Include(cr => cr.Freelancer)
            .Include(cr => cr.Messages)
            .OrderByDescending(cr => cr.CreatedAt)
            .Select(cr => ChatRoomMapper.ToResponseDTO(cr));

        return await query.ToPagedResponse(new PaginationParams());
    }
}