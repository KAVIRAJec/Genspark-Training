using Freelance_Project.Interfaces;
using Freelance_Project.Models.DTO;
using Freelance_Project.Misc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Freelance_Project.Models;
using Microsoft.AspNetCore.SignalR;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ChatController : BaseApiController
{
    private readonly IChatService _chatService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ChatController(IChatService chatService,
                          IHubContext<NotificationHub> hubContext)
    {
        _chatService = chatService;
        _hubContext = hubContext;
    }

    [HttpPost("SendMessage")]
    public async Task<IActionResult> SendMessage([FromBody] CreateChatMessageDTO dto)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (dto.SenderId != Guid.Parse(Id))
            return BadRequest("You are not authorized to send messages on behalf of other users.");

        if (dto == null || string.IsNullOrWhiteSpace(dto.Content) || dto.ChatRoomId == Guid.Empty || dto.SenderId == Guid.Empty)
            return BadRequest("Invalid message data.");

        var result = await _chatService.SendMessage(dto.Content, dto.SenderId, dto.ChatRoomId);

        var chatRoom = await _chatService.GetChatById(dto.ChatRoomId);
        if (chatRoom != null)
        {
            await _hubContext.Clients.User(chatRoom.ClientId.ToString())
                .SendAsync("ChatNotification", result);
            await _hubContext.Clients.User(chatRoom.FreelancerId.ToString())
                .SendAsync("ChatNotification", result);
        }
        return result != null ? Success(result) : NotFound("Chat room not found.");
    }

    [HttpPut("UpdateMessage/{messageId}")]
    public async Task<IActionResult> UpdateMessage(Guid messageId, [FromBody] UpdateChatMessageDTO dto)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var message = await _chatService.GetMessageById(messageId);
        if (message == null || message.SenderId != Guid.Parse(Id))
            return BadRequest("You are not authorized to update this message.");

        if (dto == null || string.IsNullOrWhiteSpace(dto.Content))
            return BadRequest("Invalid message data.");

        var result = await _chatService.UpdateMessage(messageId, dto);

        var chatRoom = await _chatService.GetChatById(message.ChatRoomId);
        if (chatRoom != null)
        {
            await _hubContext.Clients.User(chatRoom.ClientId.ToString())
                .SendAsync("ChatNotification", result);
            await _hubContext.Clients.User(chatRoom.FreelancerId.ToString())
                .SendAsync("ChatNotification", result);
        }
        return result != null ? Success(result) : NotFound("Message not found.");
    }

    [HttpPut("SetMessageRead/{messageId}")]
    public async Task<IActionResult> SetMessageRead(Guid messageId, [FromBody] ReadChatMessageDTO dto)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var message = await _chatService.GetMessageById(messageId);
        if (message == null || message.SenderId == Guid.Parse(Id))
            return BadRequest("You can't set this message as read.");

        if (dto == null || dto.ChatRoomId == Guid.Empty || dto.SenderId == Guid.Empty)
            return BadRequest("Invalid data for marking message as read.");

        var result = await _chatService.SetMessageRead(messageId, dto);

        // No notification is need for read messages
        // var chatRoom = await _chatService.GetChatById(dto.ChatRoomId);
        // if (chatRoom != null)
        // {
        //     await _hubContext.Clients.User(chatRoom.ClientId.ToString())
        //         .SendAsync("ChatNotification", result);
        //     await _hubContext.Clients.User(chatRoom.FreelancerId.ToString())
        //         .SendAsync("ChatNotification", result);
        // }
        return result != null ? Success(result) : NotFound("Message not found.");
    }

    [HttpDelete("DeleteMessage/{messageId}/ChatRoom/{chatRoomId}")]
    public async Task<IActionResult> DeleteMessage(Guid messageId, Guid chatRoomId)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var message = await _chatService.GetMessageById(messageId);
        if (message == null || message.SenderId != Guid.Parse(Id))
            return BadRequest("You are not authorized to delete this message.");

        var result = await _chatService.DeleteMessage(messageId, chatRoomId);

        var chatRoom = await _chatService.GetChatById(chatRoomId);
        if (chatRoom != null)
        {
            await _hubContext.Clients.User(chatRoom.ClientId.ToString())
                .SendAsync("ChatNotification", result);
            await _hubContext.Clients.User(chatRoom.FreelancerId.ToString())
                .SendAsync("ChatNotification", result);
        }
        return result != null ? Success(result) : NotFound("Message not found.");
    }

    [HttpGet("Messages/{chatRoomId}")]
    public async Task<IActionResult> GetMessagesByChatRoomId(Guid chatRoomId, [FromQuery] PaginationParams paginationParams)
    {
        if (chatRoomId == Guid.Empty) return BadRequest("Chat room ID cannot be empty.");

        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var chatRoom = await _chatService.GetChatById(chatRoomId);
        if (chatRoom == null || (chatRoom.ClientId != Guid.Parse(Id) && chatRoom.FreelancerId != Guid.Parse(Id)))
            return BadRequest("You are not authorized to view messages in this chat room.");
        
        var result = await _chatService.GetMessagesByChatRoomId(chatRoomId, paginationParams);
        return result != null ? Success(result) : NotFound("No messages found for the given chat room ID.");
    }

    [HttpGet("RoomByProject/{projectId}")]
    public async Task<IActionResult> GetChatRoomByProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty) return BadRequest("Project ID cannot be empty.");
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        var result = await _chatService.GetChatRoomByProjectId(projectId);
        if (result != null && (result.ClientId != Guid.Parse(Id) && result.FreelancerId != Guid.Parse(Id)))
            return BadRequest("You are not authorized to view this chat room.");
        return result != null ? Success(result) : NotFound("Chat room not found for the given project ID.");
    }

    [HttpGet("RoomsByUser/{userId}")]
    public async Task<IActionResult> GetChatRoomsByUserId(Guid userId, [FromQuery] PaginationParams paginationParams)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (userId != Guid.Parse(Id))
            return BadRequest("You are not authorized to view chat rooms for other users.");

        if (userId == Guid.Empty) return BadRequest("User ID cannot be empty.");
        
        var result = await _chatService.GetChatRoomByUserId(userId, paginationParams);
        return Success(result);
    }
}