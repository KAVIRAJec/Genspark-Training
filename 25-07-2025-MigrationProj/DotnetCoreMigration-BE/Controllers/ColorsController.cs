using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ColorsController : ControllerBase
{
    private readonly IColorService _colorService;

    public ColorsController(IColorService colorService)
    {
        _colorService = colorService;
    }

    /// <summary>
    /// Get all colors (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ColorDto>>>> GetAllColors()
    {
        try
        {
            var colors = await _colorService.GetAllColorsAsync();
            return Ok(ApiResponse<IEnumerable<ColorDto>>.SuccessResponse(colors, "Colors retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ColorDto>>.ErrorResponse("An error occurred while retrieving colors", ex.Message));
        }
    }

    /// <summary>
    /// Get all colors with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ColorDto>>>> GetAllColorsPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedColors = await _colorService.GetAllColorsPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<ColorDto>>.SuccessResponse(paginatedColors, "Paginated colors retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<ColorDto>>.ErrorResponse("An error occurred while retrieving colors", ex.Message));
        }
    }

    /// <summary>
    /// Get color by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ColorDto>>> GetColorById(int id)
    {
        try
        {
            var color = await _colorService.GetColorByIdAsync(id);
            if (color == null)
            {
                return NotFound(ApiResponse<ColorDto>.ErrorResponse("Color not found"));
            }
            return Ok(ApiResponse<ColorDto>.SuccessResponse(color, "Color retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ColorDto>.ErrorResponse("An error occurred while retrieving the color", ex.Message));
        }
    }

    /// <summary>
    /// Create a new color
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ColorDto>>> CreateColor([FromBody] CreateColorDto createColorDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ColorDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var createdColor = await _colorService.CreateColorAsync(createColorDto);
            return CreatedAtAction(nameof(GetColorById), new { id = createdColor.ColorId }, 
                ApiResponse<ColorDto>.SuccessResponse(createdColor, "Color created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ColorDto>.ErrorResponse("Failed to create color", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ColorDto>.ErrorResponse("An error occurred while creating the color", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing color
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ColorDto>>> UpdateColor(int id, [FromBody] UpdateColorDto updateColorDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ColorDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            if (id != updateColorDto.ColorId)
            {
                return BadRequest(ApiResponse<ColorDto>.ErrorResponse("Color ID in URL does not match Color ID in request body"));
            }

            var updatedColor = await _colorService.UpdateColorAsync(updateColorDto);
            return Ok(ApiResponse<ColorDto>.SuccessResponse(updatedColor, "Color updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ColorDto>.ErrorResponse("Failed to update color", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ColorDto>.ErrorResponse("An error occurred while updating the color", ex.Message));
        }
    }

    /// <summary>
    /// Delete a color (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteColor(int id)
    {
        try
        {
            var exists = await _colorService.ColorExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse.ErrorResponse("Color not found"));
            }

            var result = await _colorService.DeleteColorAsync(id);
            if (result)
            {
                return Ok(ApiResponse.SuccessResponse("Color deleted successfully"));
            }
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to delete color"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the color", ex.Message));
        }
    }

    /// <summary>
    /// Check if color name already exists
    /// </summary>
    [HttpGet("check-name")]
    public async Task<ActionResult<ApiResponse<object>>> CheckColorName([FromQuery] string name, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Color name is required"));
            }

            var exists = await _colorService.ColorNameExistsAsync(name, excludeId);
            return Ok(ApiResponse<object>.SuccessResponse(new { exists }, "Color name availability checked successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while checking color name", ex.Message));
        }
    }
}
