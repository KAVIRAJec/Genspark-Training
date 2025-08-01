using DotnetCoreMigration.Models;
using System.Security.Claims;

namespace DotnetCoreMigration.Interfaces;

public interface ITokenGenerationService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
    DateTime GetTokenExpiration(string token);
    string? GetUserIdFromToken(string token);
    string? GetEmailFromToken(string token);
    bool IsTokenExpired(string token);
}
