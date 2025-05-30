using System.ComponentModel.DataAnnotations;

namespace FAQChatBot.Models.DTOs
{
    public class FAQRequestDTO
    {
        public string Question { get; set; }
        public int UserId { get; set; }
    }
}