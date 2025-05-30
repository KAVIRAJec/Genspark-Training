using FAQChatBot.Interfaces;
using FAQChatBot.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FAQChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestDTO user)
        {
            if (user == null)
                return BadRequest("User data cannot be null.");

            var result = await _userService.CreateUser(user);
            return CreatedAtAction(nameof(GetUser), new { userId = result.Id }, result);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var user = await _userService.GetUser(userId);
            return Ok(user);
        }

        [HttpGet("all")]  
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            if (users == null || !users.Any())
                return NotFound("No users found.");
            return Ok(users);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserRequestDTO user)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            if (user == null)
                return BadRequest("User data cannot be null.");

            var result = await _userService.UpdateUser(userId, user);
            if (result == null)
                return NotFound($"User with ID {userId} not found.");
            return Ok(result);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (userId <= 0)
                return BadRequest("Invalid user ID.");

            var result = await _userService.DeleteUser(userId);
            if (result == null)
                return NotFound($"User with ID {userId} not found.");
            return Ok($"User with ID {userId} deleted successfully.");
        }
    }
}