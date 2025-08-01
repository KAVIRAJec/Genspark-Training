using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<PaginatedResponse<CategoryDto>> GetAllCategoriesPaginatedAsync(PaginationRequest request);
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
    Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto);
    Task<bool> DeleteCategoryAsync(int id);
    Task<bool> CategoryExistsAsync(int id);
    Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null);
}
