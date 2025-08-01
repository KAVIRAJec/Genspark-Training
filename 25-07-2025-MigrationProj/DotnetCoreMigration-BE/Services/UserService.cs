using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;
using BCrypt.Net;

namespace DotnetCoreMigration.Services;

public class UserService : IUserService
{
    private readonly IRepository<User, int> _userRepository;

    public UserService(IRepository<User, int> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAll();
        return UserMapper.ToDtoList(users);
    }

    public async Task<PaginatedResponse<UserDto>> GetAllUsersPaginatedAsync(PaginationRequest request)
    {
        var paginatedUsers = await _userRepository.GetAllPaginated(request);
        var userDtos = UserMapper.ToDtoList(paginatedUsers.Data);

        return new PaginatedResponse<UserDto>(
            userDtos.ToList(),
            paginatedUsers.TotalCount,
            paginatedUsers.PageNumber,
            paginatedUsers.PageSize
        );
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetById(id);
        return user == null ? null : UserMapper.ToDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.GetAll();
        var user = users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
        return user == null ? null : UserMapper.ToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if email already exists
        var existingUsers = await _userRepository.GetAll();
        if (existingUsers.Any(u => u.Email.ToLower() == createUserDto.Email.ToLower()))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Check if username already exists
        if (existingUsers.Any(u => u.UserName.ToLower() == createUserDto.UserName.ToLower()))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        var user = UserMapper.ToEntity(createUserDto);
        
        // Hash the password
        user.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        var createdUser = await _userRepository.Create(user);
        return UserMapper.ToDto(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetById(updateUserDto.UserId);
        if (existingUser == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        // Check if email already exists (excluding current user)
        var users = await _userRepository.GetAll();
        if (users.Any(u => u.UserId != updateUserDto.UserId && 
                          u.Email.ToLower() == updateUserDto.Email.ToLower()))
        {
            throw new InvalidOperationException("Email already exists.");
        }

        // Check if username already exists (excluding current user)
        if (users.Any(u => u.UserId != updateUserDto.UserId && 
                          u.UserName.ToLower() == updateUserDto.UserName.ToLower()))
        {
            throw new InvalidOperationException("Username already exists.");
        }

        UserMapper.UpdateEntity(existingUser, updateUserDto);
        var updatedUser = await _userRepository.Update(existingUser);
        return UserMapper.ToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        return await _userRepository.Delete(id);
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        var user = await _userRepository.GetById(id);
        return user != null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var users = await _userRepository.GetAll();
        return users.Any(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _userRepository.GetById(userId);
        if (user == null) return false;

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
        {
            return false;
        }

        // Hash and update new password
        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _userRepository.Update(user);
        return true;
    }

    public async Task<bool> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateProfileDto)
    {
        var user = await _userRepository.GetById(userId);
        if (user == null) return false;

        UserMapper.UpdateProfileEntity(user, updateProfileDto);
        await _userRepository.Update(user);
        return true;
    }
}
