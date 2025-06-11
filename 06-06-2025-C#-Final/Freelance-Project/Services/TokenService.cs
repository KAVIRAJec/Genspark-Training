using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Freelance_Project.Services;

public class TokenService : ITokenService
{
    protected readonly SymmetricSecurityKey _key;
    protected readonly int _expirationHours;
    protected readonly int _refreshTokenExpiresInDays;
    protected readonly FreelanceDBContext _appContext;
    protected readonly IRepository<string, User> _userRepository;
    public TokenService(IConfiguration config,
                        FreelanceDBContext appContext,
                        IRepository<string, User> userRepository)
    {
        if (config["JWT:Key"] == null || config["JWT:ExpiresInHours"] == null)
            throw new AppException("JWT configuration is missing in appSettings.json", 500);
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!));
        _expirationHours = int.Parse(config["JWT:ExpiresInHours"]!);
        _refreshTokenExpiresInDays = int.Parse(config["JWT:RefreshTokenExpiresInDays"]!);
        _appContext = appContext;
        _userRepository = userRepository;
    }
    public virtual async Task<string> GenerateToken(User user)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(_expirationHours);

        var Id = Guid.Empty;
        if (user.Role == "Client") Id = (await _appContext.Clients.FirstOrDefaultAsync(c => c.Email == user.Email))?.Id ?? Guid.Empty;
        else if (user.Role == "Freelancer") Id = (await _appContext.Freelancers.FirstOrDefaultAsync(f => f.Email == user.Email))?.Id ?? Guid.Empty;
        else throw new AppException("Invalid user role", 400);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("Id", Id.ToString())
        };
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );
        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public virtual async Task<string> GenerateRefreshToken(string email)
    {
        using var transaction = await _appContext.Database.BeginTransactionAsync();
        try
        {
            if (string.IsNullOrEmpty(email)) throw new AppException("Email is required", 400);
            var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new AppException("User not found", 404);
            if (user.DeletedAt != null || !user.IsActive) throw new AppException("User is inactive or deleted", 403);

            //remove expired token for that user
            var expiredTokens = _appContext.RefreshTokens.Where(rt => rt.Email == email && rt.Expires < DateTime.UtcNow);
            if (expiredTokens.Any()) _appContext.RefreshTokens.RemoveRange(expiredTokens);

            var refreshToken = Guid.NewGuid().ToString();
            await _appContext.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                Email = user.Email,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenExpiresInDays)
            });
            await _appContext.SaveChangesAsync();

            await _appContext.Users
            .Where(u => u.Email == email)
            .ExecuteUpdateAsync(set => set.SetProperty(u => u.LastLogin, DateTime.UtcNow));
            
            await transaction.CommitAsync();
            return refreshToken;
        } catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public virtual async Task<string> ValidateRefreshToken(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken)) throw new AppException("Refresh token is required", 400);
        var token = await _appContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken) ?? throw new AppException("Invalid refresh token", 401);
        if (token.Expires < DateTime.UtcNow) throw new AppException("Refresh token expired, please login again", 401);

        var user = await _appContext.Users.FindAsync(token.Email);
        if (user == null) throw new AppException("User not found", 404);
        if (user.DeletedAt != null || !user.IsActive) throw new AppException("User is inactive or deleted", 403);

        _appContext.RefreshTokens.Remove(token);
        await _appContext.SaveChangesAsync();

        var newRefreshToken = await GenerateRefreshToken(user.Email);
        return newRefreshToken;
}
}