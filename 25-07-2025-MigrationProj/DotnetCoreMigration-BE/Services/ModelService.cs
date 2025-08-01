using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreMigration.Services;

public class ModelService : IModelService
{
    private readonly IRepository<Model, int> _modelRepository;

    public ModelService(IRepository<Model, int> modelRepository)
    {
        _modelRepository = modelRepository;
    }

    public async Task<IEnumerable<ModelDto>> GetAllModelsAsync()
    {
        var models = await _modelRepository.GetAll();
        return ModelMapper.ToDtoList(models);
    }

    public async Task<PaginatedResponse<ModelDto>> GetAllModelsPaginatedAsync(PaginationRequest request)
    {
        var paginatedModels = await _modelRepository.GetAllPaginated(request);
        var modelDtos = paginatedModels.Data.Select(m => new ModelDto
        {
            ModelId = m.ModelId,
            Name = m.Name,
            CreatedDate = m.CreatedDate,
            IsActive = m.IsActive,
            ProductCount = m.Products?.Count(p => p.IsActive) ?? 0
        });

        return new PaginatedResponse<ModelDto>(
            modelDtos,
            paginatedModels.TotalCount,
            paginatedModels.PageNumber,
            paginatedModels.PageSize
        );
    }

    public async Task<ModelDto?> GetModelByIdAsync(int id)
    {
        var model = await _modelRepository.GetById(id);
        if (model == null) return null;

        return new ModelDto
        {
            ModelId = model.ModelId,
            Name = model.Name,
            CreatedDate = model.CreatedDate,
            IsActive = model.IsActive,
            ProductCount = model.Products?.Count(p => p.IsActive) ?? 0
        };
    }

    public async Task<ModelDto> CreateModelAsync(CreateModelDto createModelDto)
    {
        // Check if model name already exists
        var models = await _modelRepository.GetAll();
        if (models.Any(m => m.Name.ToLower() == createModelDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Model name already exists.");
        }

        var model = new Model
        {
            Name = createModelDto.Name,
            CreatedDate = DateTime.UtcNow,
            IsActive = true
        };

        var createdModel = await _modelRepository.Create(model);
        
        return new ModelDto
        {
            ModelId = createdModel.ModelId,
            Name = createdModel.Name,
            CreatedDate = createdModel.CreatedDate,
            IsActive = createdModel.IsActive,
            ProductCount = 0
        };
    }

    public async Task<ModelDto> UpdateModelAsync(UpdateModelDto updateModelDto)
    {
        var existingModel = await _modelRepository.GetById(updateModelDto.ModelId);
        if (existingModel == null)
        {
            throw new InvalidOperationException("Model not found.");
        }

        // Check if model name already exists (excluding current model)
        var models = await _modelRepository.GetAll();
        if (models.Any(m => m.ModelId != updateModelDto.ModelId && 
                           m.Name.ToLower() == updateModelDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Model name already exists.");
        }

        existingModel.Name = updateModelDto.Name;
        var updatedModel = await _modelRepository.Update(existingModel);

        return new ModelDto
        {
            ModelId = updatedModel.ModelId,
            Name = updatedModel.Name,
            CreatedDate = updatedModel.CreatedDate,
            IsActive = updatedModel.IsActive,
            ProductCount = updatedModel.Products?.Count(p => p.IsActive) ?? 0
        };
    }

    public async Task<bool> DeleteModelAsync(int id)
    {
        return await _modelRepository.Delete(id);
    }

    public async Task<bool> ModelExistsAsync(int id)
    {
        var model = await _modelRepository.GetById(id);
        return model != null;
    }

    public async Task<bool> ModelNameExistsAsync(string name, int? excludeId = null)
    {
        var models = await _modelRepository.GetAll();
        return models.Any(m => m.Name.ToLower() == name.ToLower() && 
                              (!excludeId.HasValue || m.ModelId != excludeId.Value));
    }
}
