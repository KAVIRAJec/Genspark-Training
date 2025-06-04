using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

using FirstAPI.Services;
using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Controllers;

[ApiController]
[Route("api/googleauth")]
public class GoogleAuthController : Controller
{
    private readonly ITokenService _tokenService;
    public GoogleAuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpGet("login")]
    public IActionResult GoogleLogin()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded || result.Principal == null)
            return Unauthorized("Google authentication failed.");

        var claims = result.Principal.Identities.First().Claims;
        var email = result.Principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = result.Principal.Identity?.Name ?? email ?? "GoogleUser";

        var user = new User
        {
            Username = email ?? name,
            Role = "Patient"
        };

        var token = await _tokenService.GenerateToken(user);

        return Ok(new
        {
            Message = "Login successful",
            Email = email,
            Name = name,
            Token = token
        });
    }
}
