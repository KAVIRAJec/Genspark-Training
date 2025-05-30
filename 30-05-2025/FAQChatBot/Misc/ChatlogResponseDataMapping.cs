using FAQChatBot.Models;
using FAQChatBot.Models.DTOs;

namespace FAQChatBot.Misc
{
    public class ChatlogResponseDataMapping
    {
        public ChatLogResponseDTO ToChatLogResponseDTO(ChatLog log)
        {
            ChatLogResponseDTO chatLogResponseDTO = new();
            chatLogResponseDTO.Question = log.Question;
            chatLogResponseDTO.Answer = log.Answer;
            chatLogResponseDTO.CreatedAt = log.CreatedAt;
            chatLogResponseDTO.UserId = log.UserId;
            return chatLogResponseDTO;
        }
    }
}