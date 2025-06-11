using Freelance_Project.Models;

namespace Freelance_Project.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateToken(User user);
    public Task<string> GenerateRefreshToken(string email);
    public Task<string> ValidateRefreshToken(string refreshToken);
}