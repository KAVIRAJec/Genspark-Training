using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Repositories;

namespace DotnetCoreMigration.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenGenerationService _tokenService;
    private readonly IRepository<User, int> _userRepository;
    private readonly RefreshTokenRepository _refreshTokenRepository;

    public AuthenticationService(
        ApplicationDbContext context,
        ITokenGenerationService tokenService,
        IRepository<User, int> userRepository,
        RefreshTokenRepository refreshTokenRepository)
    {
        _context = context;
        _tokenService = tokenService;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthenticationResultDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower() && u.IsActive);

            if (user == null)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var tokenExpiration = _tokenService.GetTokenExpiration(accessToken);

            // Store refresh token in database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.UserId,
                ExpirationDate = DateTime.UtcNow.AddDays(7), // 7 days expiration
                CreatedDate = DateTime.UtcNow
            };

            await _refreshTokenRepository.Create(refreshTokenEntity);

            // Create user DTO
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                Role = user.Role
            };

            return new AuthenticationResultDto
            {
                Success = true,
                Message = "Login successful.",
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpiration = tokenExpiration,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = $"Login failed: {ex.Message}"
            };
        }
    }

    public async Task<AuthenticationResultDto> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            // Check if email already exists
            var existingUserByEmail = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower() && u.IsActive);

            if (existingUserByEmail)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Email is already registered."
                };
            }

            // Check if username already exists
            var existingUserByUsername = await _context.Users
                .AnyAsync(u => u.UserName.ToLower() == registerDto.UserName.ToLower() && u.IsActive);

            if (existingUserByUsername)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Username is already taken."
                };
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Create new user
            var newUser = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                Password = hashedPassword,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Phone = registerDto.Phone,
                Address = registerDto.Address,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            // Save user
            var createdUser = await _userRepository.Create(newUser);

            // Generate tokens
            var accessToken = _tokenService.GenerateAccessToken(createdUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var tokenExpiration = _tokenService.GetTokenExpiration(accessToken);

            // Store refresh token in database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = createdUser.UserId,
                ExpirationDate = DateTime.UtcNow.AddDays(7), // 7 days expiration
                CreatedDate = DateTime.UtcNow
            };

            await _refreshTokenRepository.Create(refreshTokenEntity);

            // Create user DTO
            var userDto = new UserDto
            {
                UserId = createdUser.UserId,
                UserName = createdUser.UserName,
                Email = createdUser.Email,
                FirstName = createdUser.FirstName,
                LastName = createdUser.LastName,
                Phone = createdUser.Phone,
                Address = createdUser.Address,
                CreatedDate = createdUser.CreatedDate,
                Role = createdUser.Role
            };

            return new AuthenticationResultDto
            {
                Success = true,
                Message = "Registration successful.",
                Token = accessToken,
                RefreshToken = refreshToken,
                TokenExpiration = tokenExpiration,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = $"Registration failed: {ex.Message}"
            };
        }
    }

    public async Task<bool> LogoutAsync(string token)
    {
        try
        {
            // Get user ID from token
            var userId = _tokenService.GetUserIdFromToken(token);
            if (userId != null && int.TryParse(userId, out int userIdInt))
            {
                // Revoke all refresh tokens for this user
                await _refreshTokenRepository.RevokeAllUserTokensAsync(userIdInt);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<User?> GetCurrentUserAsync(string token)
    {
        try
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            if (userId == null || !int.TryParse(userId, out int userIdInt))
                return null;

            return await _userRepository.GetById(userIdInt);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            if (_tokenService.IsTokenExpired(token))
                return false;

            var principal = _tokenService.ValidateToken(token);
            return principal != null && await Task.FromResult(true);
        }
        catch
        {
            return false;
        }
    }

    public async Task<AuthenticationResultDto> RefreshTokenAsync(string refreshToken)
    {
        try
        {
            // Find the refresh token in database
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            // Check if token is expired
            if (DateTime.UtcNow > storedRefreshToken.ExpirationDate)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Refresh token has expired."
                };
            }

            // Check if token is revoked or used
            if (storedRefreshToken.IsRevoked || storedRefreshToken.IsUsed)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "Refresh token has been revoked."
                };
            }

            // Mark current refresh token as used
            storedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.Update(storedRefreshToken);

            // Get user
            var user = await _userRepository.GetById(storedRefreshToken.UserId);
            if (user == null || !user.IsActive)
            {
                return new AuthenticationResultDto
                {
                    Success = false,
                    Message = "User not found or inactive."
                };
            }

            // Generate new tokens
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var tokenExpiration = _tokenService.GetTokenExpiration(newAccessToken);

            // Store new refresh token in database
            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = user.UserId,
                ExpirationDate = DateTime.UtcNow.AddDays(7), // 7 days expiration
                CreatedDate = DateTime.UtcNow
            };

            await _refreshTokenRepository.Create(newRefreshTokenEntity);

            // Create user DTO
            var userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                Role = user.Role
            };

            return new AuthenticationResultDto
            {
                Success = true,
                Message = "Token refreshed successfully.",
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = tokenExpiration,
                User = userDto
            };
        }
        catch (Exception ex)
        {
            return new AuthenticationResultDto
            {
                Success = false,
                Message = $"Token refresh failed: {ex.Message}"
            };
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _userRepository.GetById(userId);
    }
}
