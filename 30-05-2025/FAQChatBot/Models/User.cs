using System.Collections.Generic;

namespace FAQChatBot.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ChatLog>? ChatLogs { get; set; }
    }
}
