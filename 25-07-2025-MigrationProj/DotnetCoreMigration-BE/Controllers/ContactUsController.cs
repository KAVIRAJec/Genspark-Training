using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;
using System.Security.Claims;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactUsController : ControllerBase
{
    private readonly IContactUsService _contactUsService;

    public ContactUsController(IContactUsService contactUsService)
    {
        _contactUsService = contactUsService;
    }

    /// <summary>
    /// Get all contact us messages (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContactUsDto>>>> GetAllContactUs()
    {
        try
        {
            var contactUs = await _contactUsService.GetAllContactUsAsync();
            return Ok(ApiResponse<IEnumerable<ContactUsDto>>.SuccessResponse(contactUs, "Contact us messages retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ContactUsDto>>.ErrorResponse("An error occurred while retrieving contact us messages", ex.Message));
        }
    }

    /// <summary>
    /// Get all contact us messages with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ContactUsDto>>>> GetAllContactUsPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedContactUs = await _contactUsService.GetAllContactUsPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<ContactUsDto>>.SuccessResponse(paginatedContactUs, "Paginated contact us messages retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<ContactUsDto>>.ErrorResponse("An error occurred while retrieving contact us messages", ex.Message));
        }
    }

    /// <summary>
    /// Get contact us message by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ContactUsDto>>> GetContactUsById(int id)
    {
        try
        {
            var contactUs = await _contactUsService.GetContactUsByIdAsync(id);
            if (contactUs == null)
            {
                return NotFound(ApiResponse<ContactUsDto>.ErrorResponse("Contact us message not found", $"No contact us message found with ID {id}"));
            }
            return Ok(ApiResponse<ContactUsDto>.SuccessResponse(contactUs, "Contact us message retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContactUsDto>.ErrorResponse("An error occurred while retrieving the contact us message", ex.Message));
        }
    }
    /// <summary>
    /// Get contact us messages by userId
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContactUsDto>>>> GetContactUsByUserId(string userId)
    {
        try
        {
            var Id = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (Id != userId)
            {
                return Unauthorized(ApiResponse<IEnumerable<ContactUsDto>>.ErrorResponse("Unauthorized access", "You do not have permission to access this resource"));
            }
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var contactUs = await _contactUsService.GetContactUsByEmailAsync(email);
            if (contactUs == null || !contactUs.Any())
            {
                return NotFound(ApiResponse<IEnumerable<ContactUsDto>>.ErrorResponse("No contact us messages found for the user", $"No contact us messages found for user with ID {userId}"));
            }
            return Ok(ApiResponse<IEnumerable<ContactUsDto>>.SuccessResponse(contactUs, "Contact us messages retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ContactUsDto>>.ErrorResponse("An error occurred while retrieving contact us messages", ex.Message));
        }
    }

    /// <summary>
    /// Get unread contact us messages
    /// </summary>
    [HttpGet("unread")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ContactUsDto>>>> GetUnreadContactUs()
    {
        try
        {
            var unreadContactUs = await _contactUsService.GetUnreadContactUsAsync();
            return Ok(ApiResponse<IEnumerable<ContactUsDto>>.SuccessResponse(unreadContactUs, "Unread contact us messages retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ContactUsDto>>.ErrorResponse("An error occurred while retrieving unread contact us messages", ex.Message));
        }
    }

    /// <summary>
    /// Create a new contact us message
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactUsDto>>> CreateContactUs([FromBody] CreateContactUsDto createContactUsDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ContactUsDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var contactUs = await _contactUsService.CreateContactUsAsync(createContactUsDto);
            return CreatedAtAction(nameof(GetContactUsById), new { id = contactUs.ContactId }, ApiResponse<ContactUsDto>.SuccessResponse(contactUs, "Contact us message created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContactUsDto>.ErrorResponse("An error occurred while creating the contact us message", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing contact us message
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ContactUsDto>>> UpdateContactUs(int id, [FromBody] UpdateContactUsDto updateContactUsDto)
    {
        try
        {
            if (id != updateContactUsDto.ContactId)
            {
                return BadRequest(ApiResponse<ContactUsDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ContactUsDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var exists = await _contactUsService.ContactUsExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<ContactUsDto>.ErrorResponse("Contact us message not found", $"No contact us message found with ID {id}"));
            }

            var contactUs = await _contactUsService.UpdateContactUsAsync(updateContactUsDto);
            return Ok(ApiResponse<ContactUsDto>.SuccessResponse(contactUs, "Contact us message updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<ContactUsDto>.ErrorResponse("Contact us message not found", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ContactUsDto>.ErrorResponse("An error occurred while updating the contact us message", ex.Message));
        }
    }

    /// <summary>
    /// Mark contact us message as read
    /// </summary>
    [HttpPatch("{id}/mark-read")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkAsRead(int id)
    {
        try
        {
            var result = await _contactUsService.MarkAsReadAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Contact us message not found", $"No contact us message found with ID {id}"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Contact us message marked as read successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while marking the contact us message as read", ex.Message));
        }
    }

    /// <summary>
    /// Delete a contact us message (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteContactUs(int id)
    {
        try
        {
            var exists = await _contactUsService.ContactUsExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Contact us message not found", $"No contact us message found with ID {id}"));
            }

            var result = await _contactUsService.DeleteContactUsAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Contact us message deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the contact us message", ex.Message));
        }
    }
}
