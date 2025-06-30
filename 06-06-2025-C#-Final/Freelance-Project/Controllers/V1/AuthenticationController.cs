using System.Security.Claims;
using Freelance_Project.Interfaces;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthenticationController : BaseApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
    {
        if (loginRequest == null) return BadRequest("Login request cannot be null");

        var response = await _authenticationService.Login(loginRequest);

        //set HttpOnly cookie for the refresh token
        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = response.RefreshExpiresAt,
        });
        response.RefreshToken = string.Empty; // Clear the refresh token from the response to avoid sending it in the body
        return response != null ? Success(response) : BadRequest("Login failed");
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return BadRequest("Email not found in claims");

        var result = await _authenticationService.Logout(email);
        return result ? Success("Logout successful") : BadRequest("Logout failed");
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken)) return BadRequest("Refresh token cannot be null or empty");

        var response = await _authenticationService.RefreshToken(refreshToken);
        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = response.RefreshExpiresAt
        });

        response.RefreshToken = string.Empty;
        return response != null ? Success(response) : BadRequest("Token refresh failed");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetDetails()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(email)) return BadRequest("Email not found in claims");

        if (role == "Freelancer")
        {
            var result = await _authenticationService.GetDetails<FreelancerResponseDTO>(email);
            return Success(result);
        }
        else if (role == "Client")
        {
            var result = await _authenticationService.GetDetails<ClientResponseDTO>(email);
            return Success(result);
        }
        return BadRequest("Invalid role in token.");
    }
}