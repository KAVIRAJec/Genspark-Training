using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Get all users (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users, "Users retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResponse("An error occurred while retrieving users", ex.Message));
        }
    }

    /// <summary>
    /// Get all users with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<UserDto>>>> GetAllUsersPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedUsers = await _userService.GetAllUsersPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<UserDto>>.SuccessResponse(paginatedUsers, "Paginated users retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<UserDto>>.ErrorResponse("An error occurred while retrieving users", ex.Message));
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found", $"No user found with ID {id}"));
            }
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving the user", ex.Message));
        }
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found", $"No user found with email {email}"));
            }
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving the user", ex.Message));
        }
    }


    /// <summary>
    /// Update an existing user
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (id != updateUserDto.UserId)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var exists = await _userService.UserExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found", $"No user found with ID {id}"));
            }

            var user = await _userService.UpdateUserAsync(updateUserDto);
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<UserDto>.ErrorResponse("User update failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while updating the user", ex.Message));
        }
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPatch("{id}/profile")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateUserProfile(int id, [FromBody] UpdateUserProfileDto updateProfileDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var result = await _userService.UpdateUserProfileAsync(id, updateProfileDto);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found", $"No user found with ID {id}"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User profile updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while updating the user profile", ex.Message));
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    [HttpPatch("{id}/change-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            if (id != changePasswordDto.UserId)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var result = await _userService.ChangePasswordAsync(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Password change failed", "Current password is incorrect or user not found"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Password changed successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while changing the password", ex.Message));
        }
    }

    /// <summary>
    /// Check if email exists
    /// </summary>
    [HttpGet("check-email/{email}")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckEmailExists(string email)
    {
        try
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(ApiResponse<bool>.SuccessResponse(exists, exists ? "Email exists" : "Email is available"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while checking email availability", ex.Message));
        }
    }

    /// <summary>
    /// Delete a user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
    {
        try
        {
            var exists = await _userService.UserExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found", $"No user found with ID {id}"));
            }

            var result = await _userService.DeleteUserAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the user", ex.Message));
        }
    }
}
