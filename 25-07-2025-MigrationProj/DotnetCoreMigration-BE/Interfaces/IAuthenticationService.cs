using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthenticationResultDto> RegisterAsync(RegisterDto registerDto);
    Task<bool> LogoutAsync(string token);
    Task<User?> GetCurrentUserAsync(string token);
    Task<User?> GetUserByIdAsync(int userId);
    Task<bool> ValidateTokenAsync(string token);
    Task<AuthenticationResultDto> RefreshTokenAsync(string refreshToken);
}
