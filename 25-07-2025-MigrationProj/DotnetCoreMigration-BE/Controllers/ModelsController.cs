using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly IModelService _modelService;

    public ModelsController(IModelService modelService)
    {
        _modelService = modelService;
    }

    /// <summary>
    /// Get all models (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ModelDto>>>> GetAllModels()
    {
        try
        {
            var models = await _modelService.GetAllModelsAsync();
            return Ok(ApiResponse<IEnumerable<ModelDto>>.SuccessResponse(models, "Models retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ModelDto>>.ErrorResponse("An error occurred while retrieving models", ex.Message));
        }
    }

    /// <summary>
    /// Get all models with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ModelDto>>>> GetAllModelsPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedModels = await _modelService.GetAllModelsPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<ModelDto>>.SuccessResponse(paginatedModels, "Paginated models retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<ModelDto>>.ErrorResponse("An error occurred while retrieving models", ex.Message));
        }
    }

    /// <summary>
    /// Get model by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ModelDto>>> GetModelById(int id)
    {
        try
        {
            var model = await _modelService.GetModelByIdAsync(id);
            if (model == null)
            {
                return NotFound(ApiResponse<ModelDto>.ErrorResponse("Model not found"));
            }
            return Ok(ApiResponse<ModelDto>.SuccessResponse(model, "Model retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ModelDto>.ErrorResponse("An error occurred while retrieving the model", ex.Message));
        }
    }

    /// <summary>
    /// Create a new model
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ModelDto>>> CreateModel([FromBody] CreateModelDto createModelDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ModelDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            var createdModel = await _modelService.CreateModelAsync(createModelDto);
            return CreatedAtAction(nameof(GetModelById), new { id = createdModel.ModelId }, 
                ApiResponse<ModelDto>.SuccessResponse(createdModel, "Model created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ModelDto>.ErrorResponse("Failed to create model", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ModelDto>.ErrorResponse("An error occurred while creating the model", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing model
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ModelDto>>> UpdateModel(int id, [FromBody] UpdateModelDto updateModelDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ModelDto>.ValidationErrorResponse(ModelState.GetValidationErrors()));
            }

            if (id != updateModelDto.ModelId)
            {
                return BadRequest(ApiResponse<ModelDto>.ErrorResponse("Model ID in URL does not match Model ID in request body"));
            }

            var updatedModel = await _modelService.UpdateModelAsync(updateModelDto);
            return Ok(ApiResponse<ModelDto>.SuccessResponse(updatedModel, "Model updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ModelDto>.ErrorResponse("Failed to update model", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ModelDto>.ErrorResponse("An error occurred while updating the model", ex.Message));
        }
    }

    /// <summary>
    /// Delete a model (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteModel(int id)
    {
        try
        {
            var exists = await _modelService.ModelExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse.ErrorResponse("Model not found"));
            }

            var result = await _modelService.DeleteModelAsync(id);
            if (result)
            {
                return Ok(ApiResponse.SuccessResponse("Model deleted successfully"));
            }
            return StatusCode(500, ApiResponse.ErrorResponse("Failed to delete model"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.ErrorResponse("An error occurred while deleting the model", ex.Message));
        }
    }

    /// <summary>
    /// Check if model name already exists
    /// </summary>
    [HttpGet("check-name")]
    public async Task<ActionResult<ApiResponse<object>>> CheckModelName([FromQuery] string name, [FromQuery] int? excludeId = null)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Model name is required"));
            }

            var exists = await _modelService.ModelNameExistsAsync(name, excludeId);
            return Ok(ApiResponse<object>.SuccessResponse(new { exists }, "Model name availability checked successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while checking model name", ex.Message));
        }
    }
}
