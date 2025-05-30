
namespace FAQChatBot.Models.DTOs
{
    public class ChatLogResponseDTO
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int FAQId { get; set; }
    }
}