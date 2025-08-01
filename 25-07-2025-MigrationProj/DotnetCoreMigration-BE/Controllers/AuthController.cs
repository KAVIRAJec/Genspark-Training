using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthenticationResultDto>>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var result = await _authService.LoginAsync(loginDto);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ErrorResponse("Login failed", result.Message));
            }

            return Ok(ApiResponse<AuthenticationResultDto>.SuccessResponse(result, "Login successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthenticationResultDto>.ErrorResponse("An error occurred during login", ex.Message));
        }
    }

    /// <summary>
    /// Register new user
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthenticationResultDto>>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var result = await _authService.RegisterAsync(registerDto);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ErrorResponse("Registration failed", result.Message));
            }

            return Ok(ApiResponse<AuthenticationResultDto>.SuccessResponse(result, "Registration successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthenticationResultDto>.ErrorResponse("An error occurred during registration", ex.Message));
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthenticationResultDto>>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            
            if (!result.Success)
            {
                return BadRequest(ApiResponse<AuthenticationResultDto>.ErrorResponse("Token refresh failed", result.Message));
            }

            return Ok(ApiResponse<AuthenticationResultDto>.SuccessResponse(result, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<AuthenticationResultDto>.ErrorResponse("An error occurred during token refresh", ex.Message));
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        try
        {
            var refreshToken = Request.Headers["RefreshToken"].FirstOrDefault();
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(ApiResponse.ErrorResponse("Refresh token is required"));
            }

            var result = await _authService.LogoutAsync(refreshToken);
            
            if (!result)
            {
                return BadRequest(ApiResponse.ErrorResponse("Logout failed"));
            }

            return Ok(ApiResponse.SuccessResponse("Logout successful"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred during logout", ex.Message));
        }
    }

    /// <summary>
    /// Validate token
    /// </summary>
    [HttpPost("validate")]
    public async Task<ActionResult<ApiResponse<object>>> ValidateToken([FromBody] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Token is required"));
            }

            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(ApiResponse<object>.SuccessResponse(new { isValid }, "Token validation completed"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during token validation", ex.Message));
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Invalid user ID in token"));
            }

            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
            }

            // Map User entity to UserDto
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
                IsActive = user.IsActive,
                Role = user.Role,
                OrderCount = user.Orders?.Count(o => o.IsActive) ?? 0,
                ProductCount = user.Products?.Count(p => p.IsActive) ?? 0,
                NewsCount = user.News?.Count(n => n.IsActive) ?? 0
            };

            return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, "User information retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving user information", ex.Message));
        }
    }
}
