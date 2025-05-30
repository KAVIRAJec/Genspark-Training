using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using FAQChatBot.Interfaces;
using FAQChatBot.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FAQChatBot.Repositories
{
    public class ChatLogRepository : Repository<int, ChatLog>
    {
        public ChatLogRepository(FAQContext context) : base(context)
        {
        }

        public override async Task<ChatLog> Get(int id)
        {
            var chatLog = await _context.ChatLogs.SingleOrDefaultAsync(c => c.Id == id);
            return chatLog ?? throw new KeyNotFoundException($"Chat log with ID {id} not found.");
        }

        public override async Task<IEnumerable<ChatLog>> GetAll()
        {
            var chatLogs = await _context.ChatLogs.ToListAsync();
            if (chatLogs == null || chatLogs.Count == 0)
                throw new KeyNotFoundException("No chat logs in the database.");
            return chatLogs;
        }
    }
}