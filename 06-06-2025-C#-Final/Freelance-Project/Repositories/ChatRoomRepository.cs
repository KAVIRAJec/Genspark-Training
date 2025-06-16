using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class ChatRoomRepository : Repository<Guid, ChatRoom>
{
    public ChatRoomRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<ChatRoom> Get(Guid key)
    {
        var chatRoom = await _appContext.ChatRooms
            .Include(cr => cr.Client)
            .Include(cr => cr.Freelancer)
            .Include(cr => cr.Messages)
            .FirstOrDefaultAsync(cr => cr.Id == key);

        return chatRoom ?? throw new AppException($"ChatRoom with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<ChatRoom>> GetAll()
    {
        return await _appContext.ChatRooms
            .Include(cr => cr.Client)
            .Include(cr => cr.Freelancer)
            .Include(cr => cr.Messages)
            .ToListAsync();
    }

    public override async Task<ChatRoom> Delete(Guid key)
    {
        var chatRoom = await Get(key);
        if (chatRoom != null)
        {
            chatRoom.IsActive = false; // Soft delete
            chatRoom.DeletedAt = DateTime.UtcNow;
            chatRoom = await Update(chatRoom.Id, chatRoom);
            if (chatRoom == null) throw new AppException($"Failed to delete chat room.", 500);

            var messages = await _appContext.ChatMessages
                .Where(m => m.ChatRoomId == chatRoom.Id)
                .ToListAsync();

            foreach (var message in messages)
            {
                message.IsActive = false; // Soft delete
                message.DeletedAt = DateTime.UtcNow;
                _appContext.ChatMessages.Update(message);
            }
            return chatRoom;
        }
        throw new AppException($"ChatRoom with ID {key} not found.", 404);
    }
}