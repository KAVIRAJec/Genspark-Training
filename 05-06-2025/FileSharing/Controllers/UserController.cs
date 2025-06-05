using FileApp.Services;
using FileApp.Interfaces;
using FileApp.Models;
using FileApp.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileApp.Controllers;

using FileApp.Models;
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] UserRequestDto user)
    {
        try
        {
            var result = await _userService.CreateUser(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto user)
    {
        try
        {
            var result = await _userService.LoginUser(user);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("{name}")]
    public async Task<IActionResult> GetUser(string name)
    {
        try
        {
            var result = await _userService.GetUser(name);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}