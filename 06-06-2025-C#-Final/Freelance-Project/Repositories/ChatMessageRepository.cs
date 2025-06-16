using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class ChatMessageRepository : Repository<Guid, ChatMessage>
{
    public ChatMessageRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<ChatMessage> Get(Guid key)
    {
        var message = await _appContext.ChatMessages
            .Include(cm => cm.ChatRoom)
            .FirstOrDefaultAsync(cm => cm.Id == key);

        return message ?? throw new AppException($"ChatMessage with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<ChatMessage>> GetAll()
    {
        return await _appContext.Set<ChatMessage>()
            .Include(cm => cm.ChatRoom)
            .ToListAsync();
    }

    public override async Task<ChatMessage> Delete(Guid key)
    {
        var message = await Get(key);
        if (message != null)
        {
            message.IsActive = false; // Soft delete
            message.DeletedAt = DateTime.UtcNow;
            _appContext.ChatMessages.Update(message);
            await _appContext.SaveChangesAsync();
            return message;
        }
        throw new AppException($"ChatMessage with ID {key} not found.", 404);
    }
}