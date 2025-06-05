using FileApp.Models.DTOs;
using FileApp.Models;
using FileApp.Interfaces;
using FileApp.Repositories;

namespace FileApp.Services;

public class UserService : IUserService
{
    private readonly IRepository<string, User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IEncryptionService _encryptionService;


    public UserService(IRepository<string, User> userRepository,
                        ITokenService tokenService,
                        IEncryptionService encryptionService
                        )
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _encryptionService = encryptionService;
    }

    public async Task<User> CreateUser(UserRequestDto userRequestDto)
    {
        try
        {
            // var existingUser = await _userRepository.Get(userRequestDto.UserName);
            // if (existingUser != null) throw new Exception("User already exists");
            if(userRequestDto.Role != "Admin" && userRequestDto.Role != "User")
                throw new ArgumentException("Invalid role. Only 'Admin' or 'User' are allowed.");
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = userRequestDto.Password
            });

            var newUser = new User
            {
                UserName = userRequestDto.UserName,
                Email = userRequestDto.Email,
                Password = encryptedData.EncryptedData,
                HashKey = encryptedData.HashKey,
                Role = userRequestDto.Role
            };

            return await _userRepository.Add(newUser);
        }
        catch (Exception ex)
        {
            throw new Exception($"User creation failed: {ex.Message}", ex);
        }
    }

    public async Task<LoginResponseDto> LoginUser(LoginRequestDto user)
    {
        try
        {
            var dbUser = await _userRepository.Get(user.UserName);
            if (dbUser == null) throw new Exception("No such user");
            var encryptedData = await _encryptionService.EncryptData(new EncryptModel
            {
                Data = user.Password,
                HashKey = dbUser.HashKey ?? null
            });
            for (int i = 0; i < encryptedData.EncryptedData.Length; i++)
            {
                if (encryptedData.EncryptedData[i] != dbUser.Password[i])
                {
                    throw new Exception("Invalid password");
                }
            }
            var token = await _tokenService.GenerateToken(dbUser);
            return new LoginResponseDto
            {
                UserName = user.UserName,
                Token = token,
                Role = dbUser.Role,
                Email = dbUser.Email
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Authentication failed: {ex.Message}", ex);
        }
    }
    public async Task<User> GetUser(string name)
    {
        try
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Username cannot be null or empty.");
            var user = await _userRepository.Get(name);
            if (user == null) throw new Exception("User not found.");
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve user: {ex.Message}", ex);
        }
    }
    public async Task<IEnumerable<User>> GetAllUsers()
    {
        try
        {
            return await _userRepository.GetAll();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to retrieve users: {ex.Message}", ex);
        }
    }
    public async Task<User> UpdateUser(int id, UserRequestDto userRequestDto)
    {
        throw new NotImplementedException("UpdateUser method is not implemented yet.");
    }
    public async Task DeleteUser(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Invalid username.");
            var user = await _userRepository.Get(username);
            if (user == null) throw new Exception("User not found.");
            await _userRepository.Delete(username);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete user: {ex.Message}", ex);
        }
    }
}