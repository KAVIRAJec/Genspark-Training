namespace FAQChatBot.Models
{
    public class ChatLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; }
        public int? FAQId { get; set; }
        public FAQ FAQ { get; set; }
    }
}
