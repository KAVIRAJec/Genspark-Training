using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IModelService
{
    Task<IEnumerable<ModelDto>> GetAllModelsAsync();
    Task<PaginatedResponse<ModelDto>> GetAllModelsPaginatedAsync(PaginationRequest request);
    Task<ModelDto?> GetModelByIdAsync(int id);
    Task<ModelDto> CreateModelAsync(CreateModelDto createModelDto);
    Task<ModelDto> UpdateModelAsync(UpdateModelDto updateModelDto);
    Task<bool> DeleteModelAsync(int id);
    Task<bool> ModelExistsAsync(int id);
    Task<bool> ModelNameExistsAsync(string name, int? excludeId = null);
}
