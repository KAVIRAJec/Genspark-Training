using System.Threading.Tasks;
using FAQChatBot.Models.DTOs;

namespace FAQChatBot.Interfaces
{
    public interface IFAQService
    {
        Task<FAQResponseDTO> GetAnswer(FAQRequestDTO request);
        Task<IEnumerable<ChatLogResponseDTO>> GetAllChatLogs();
    }
}