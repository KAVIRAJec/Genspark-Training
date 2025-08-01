using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get all categories (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllCategories()
    {
        try
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(ApiResponse<IEnumerable<CategoryDto>>.SuccessResponse(categories, "Categories retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories", ex.Message));
        }
    }

    /// <summary>
    /// Get all categories with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<CategoryDto>>>> GetAllCategoriesPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedCategories = await _categoryService.GetAllCategoriesPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<CategoryDto>>.SuccessResponse(paginatedCategories, "Paginated categories retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<CategoryDto>>.ErrorResponse("An error occurred while retrieving categories", ex.Message));
        }
    }

    /// <summary>
    /// Get category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategoryById(int id)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(ApiResponse<CategoryDto>.ErrorResponse("Category not found"));
            }
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category, "Category retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while retrieving the category", ex.Message));
        }
    }

    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<CategoryDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, 
                ApiResponse<CategoryDto>.SuccessResponse(createdCategory, "Category created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Failed to create category", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while creating the category", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateCategoryDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<CategoryDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            if (id != updateCategoryDto.CategoryId)
            {
                return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Category ID in URL does not match Category ID in request body"));
            }

            var updatedCategory = await _categoryService.UpdateCategoryAsync(updateCategoryDto);
            return Ok(ApiResponse<CategoryDto>.SuccessResponse(updatedCategory, "Category updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<CategoryDto>.ErrorResponse("Failed to update category", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<CategoryDto>.ErrorResponse("An error occurred while updating the category", ex.Message));
        }
    }

    /// <summary>
    /// Delete a category (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCategory(int id)
    {
        try
        {
            var exists = await _categoryService.CategoryExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse.ErrorResponse("Category not found"));
            }

            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result)
            {
                return Ok(ApiResponse.SuccessResponse("Category deleted successfully"));
            }
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to delete category"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the category", ex.Message));
        }
    }

    /// <summary>
    /// Check if category name already exists
    /// </summary>
    [HttpGet("check-name")]
    public async Task<ActionResult<ApiResponse<object>>> CheckCategoryName([FromQuery] string name, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Category name is required"));
            }

            var exists = await _categoryService.CategoryNameExistsAsync(name, excludeId);
            return Ok(ApiResponse<object>.SuccessResponse(new { exists }, "Category name availability checked successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while checking category name", ex.Message));
        }
    }
}
