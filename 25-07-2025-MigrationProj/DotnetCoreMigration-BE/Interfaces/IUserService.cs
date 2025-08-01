using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<PaginatedResponse<UserDto>> GetAllUsersPaginatedAsync(PaginationRequest request);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UserExistsAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateProfileDto);
}
