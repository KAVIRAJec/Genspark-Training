using FAQChatBot.Models.DTOs;
using System.Threading.Tasks;

namespace FAQChatBot.Interfaces
{
    public interface IUserService
    {
        public Task<UserResponseDTO> CreateUser(UserRequestDTO user);
        public Task<UserResponseDTO> GetUser(int userId);
        public Task<IEnumerable<UserResponseDTO>> GetAllUsers();
        public Task<UserResponseDTO> UpdateUser(int userId, UserRequestDTO user);
        public Task<UserResponseDTO> DeleteUser(int userId);
    }
}