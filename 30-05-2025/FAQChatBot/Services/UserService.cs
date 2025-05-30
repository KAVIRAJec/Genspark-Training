using System.Collections.Generic;
using System.Threading.Tasks;
using FAQChatBot.Interfaces;
using FAQChatBot.Models.DTOs;
using FAQChatBot.Models;
using FAQChatBot.Repositories;

namespace FAQChatBot.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepository;

        public UserService(IRepository<int, User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDTO> CreateUser(UserRequestDTO user)
        {
            try{
                if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
                    throw new ArgumentException("Name and Email cannot be null or empty.");
                var userEntity = new User
                {
                    Name = user.Name,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                };
                await _userRepository.Create(userEntity);
                return new UserResponseDTO
                {
                    Id = userEntity.Id,
                    Name = userEntity.Name,
                    Email = userEntity.Email
                };
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Invalid user data: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> GetUser(int userId)
        {
            try{
                if (userId <= 0) throw new ArgumentException("User ID must be greater than zero.");
                var user = await _userRepository.Get(userId);
                if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found.");
                return new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };
            }
            catch (ArgumentException ex)
            {
                throw new Exception($"Invalid user ID: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsers()
        {
            try{
                var result = await _userRepository.GetAll();
                if (result == null || !result.Any())
                    throw new KeyNotFoundException("No users found in the database.");
                List<UserResponseDTO> userResponses = new List<UserResponseDTO>();
                foreach (var user in result)
                {
                    userResponses.Add(new UserResponseDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email
                    });
                }
                return userResponses;
            } catch (KeyNotFoundException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResponseDTO> UpdateUser(int userId, UserRequestDTO user)
        {
            try{
                if (userId <= 0) throw new ArgumentException("User ID must be greater than zero.");
                if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email))
                    throw new ArgumentException("Name and Email cannot be null or empty.");
                var existingUser = await _userRepository.Get(userId);
                if (existingUser == null) throw new KeyNotFoundException($"User with ID {userId} not found.");
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.CreatedAt = DateTime.UtcNow;
                await _userRepository.Update(userId, existingUser);
                var userResponse = new UserResponseDTO
                {
                    Id = userId,
                    Name = user.Name,
                    Email = user.Email
                };
                return userResponse;
            } catch (ArgumentException ex)
            {
                throw new Exception($"Invalid user data: {ex.Message}");
            }
        }

        public async Task<UserResponseDTO> DeleteUser(int userId)
        {
            try{
                if (userId <= 0) throw new ArgumentException("User ID must be greater than zero.");
                var user = await _userRepository.Get(userId);
                if (user == null) throw new KeyNotFoundException($"User with ID {userId} not found.");
                await _userRepository.Delete(userId);
                return new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email
                };
            } catch (ArgumentException ex)
            {
                throw new Exception($"Invalid user ID: {ex.Message}");
            } catch (KeyNotFoundException ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}