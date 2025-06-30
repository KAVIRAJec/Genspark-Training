using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<string, User> _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IHashingService _hashingService;
    private readonly IConfiguration _config;
    private readonly FreelanceDBContext _appContext;

    public AuthenticationService(IRepository<string, User> userRepository,
                                ITokenService tokenService,
                                IHashingService hashingService,
                                IConfiguration config,
                                FreelanceDBContext appContext)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _hashingService = hashingService;
        _config = config;
        _appContext = appContext;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO user)
    {
        if (user == null) throw new AppException("User login request cannot be null", 400);
        user.Email = user.Email.ToLower();
        var existingUser = await _userRepository.Get(user.Email);
        if (existingUser == null || existingUser.IsActive == false)
            throw new AppException("User not found or inactive", 400);

        if (!await _hashingService.VerifyHash(new HashingModel
        {
            Data = user.Password,
            HashedData = existingUser.Password,
            HashKey = existingUser.HashKey
        }))
            throw new AppException("Invalid password", 400);

        var token = await _tokenService.GenerateToken(existingUser);
        var refreshToken = await _tokenService.GenerateRefreshToken(existingUser.Email);
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            throw new AppException("Unable to generate token/refresh token", 500);

        var response = new LoginResponseDTO
        {
            Email = existingUser.Email,
            Role = existingUser.Role,
            Token = token,
            RefreshToken = refreshToken,
            RefreshExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:RefreshTokenExpiresInDays"]!))
        };
        return response;
    }

    public async Task<bool> Logout(string email)
    {
        if (string.IsNullOrEmpty(email)) throw new AppException("Email is required", 400);
        email = email.ToLower();
        var user = await _userRepository.Get(email);
        if (user == null) throw new AppException("User not found", 404);
        var expiredTokens = _appContext.RefreshTokens.Where(rt => rt.Email == email);
        if (expiredTokens.Any())
        {
            _appContext.RefreshTokens.RemoveRange(expiredTokens);
        }
        await _appContext.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponseDTO> RefreshToken(string refreshToken)
    {
        if (refreshToken == null) throw new AppException("Refresh token is required", 400);
        var token = await _tokenService.ValidateRefreshToken(refreshToken);
        if (string.IsNullOrEmpty(token)) throw new AppException("Unable to refresh token", 401);
        var email = _appContext.RefreshTokens.Where(rt => rt.Token == token).Select(rt => rt.Email).FirstOrDefault();
        if (string.IsNullOrEmpty(email)) throw new AppException("Invalid refresh token", 401);

        var existingUser = await _userRepository.Get(email);
        if (existingUser == null || existingUser.IsActive == false)
            throw new AppException("User not found or inactive", 404);
        var response = new LoginResponseDTO
        {
            Email = existingUser.Email,
            Role = existingUser.Role,
            Token = await _tokenService.GenerateToken(existingUser),
            RefreshToken = token,
            RefreshExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:RefreshTokenExpiresInDays"]!))
        };
        return response;
    }

    public async Task<T> GetDetails<T>(string email) where T : class
    {
        if (string.IsNullOrEmpty(email)) throw new AppException("Email is required", 400);
        email = email.ToLower();
        var user = await _userRepository.Get(email);
        if (user == null || user.IsActive == false)
            throw new AppException("User not found or inactive", 404);

        if (user.Role == "Freelancer")
        {
            var freelancer = await _appContext.Freelancers.Include(f => f.Skills).FirstOrDefaultAsync(f => f.Email == email);
            if (freelancer == null) throw new AppException("Freelancer details not found", 404);
            var response = FreelancerMapper.ToResponseDTO(freelancer);
            return response as T ?? throw new AppException("Failed to map freelancer details", 500);
        }
        if (user.Role == "Client")
        {
            var client = await _appContext.Clients.Include(c => c.Projects).FirstOrDefaultAsync(c => c.Email == email);
            if (client == null) throw new AppException("Client details not found", 404);
            var response = ClientMapper.ToResponseDTO(client);
            return response as T ?? throw new AppException("Failed to map client details", 500);
        }
        throw new AppException("Invalid user role", 400);
    }
}