using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreMigration.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<Category, int> _categoryRepository;

    public CategoryService(IRepository<Category, int> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAll();
        return CategoryMapper.ToDtoList(categories);
    }

    public async Task<PaginatedResponse<CategoryDto>> GetAllCategoriesPaginatedAsync(PaginationRequest request)
    {
        var paginatedCategories = await _categoryRepository.GetAllPaginated(request);
        var categoryDtos = CategoryMapper.ToDtoList(paginatedCategories.Data);

        return new PaginatedResponse<CategoryDto>(
            categoryDtos.ToList(),
            paginatedCategories.TotalCount,
            paginatedCategories.PageNumber,
            paginatedCategories.PageSize
        );
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetById(id);
        return category == null ? null : CategoryMapper.ToDto(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
    {
        // Check if category name already exists
        var categories = await _categoryRepository.GetAll();
        if (categories.Any(c => c.Name.ToLower() == createCategoryDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Category name already exists.");
        }

        var category = CategoryMapper.ToEntity(createCategoryDto);
        var createdCategory = await _categoryRepository.Create(category);
        return CategoryMapper.ToDto(createdCategory);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(UpdateCategoryDto updateCategoryDto)
    {
        var existingCategory = await _categoryRepository.GetById(updateCategoryDto.CategoryId);
        if (existingCategory == null)
        {
            throw new InvalidOperationException("Category not found.");
        }

        // Check if category name already exists (excluding current category)
        var categories = await _categoryRepository.GetAll();
        if (categories.Any(c => c.CategoryId != updateCategoryDto.CategoryId && 
                               c.Name.ToLower() == updateCategoryDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Category name already exists.");
        }

        CategoryMapper.UpdateEntity(existingCategory, updateCategoryDto);
        var updatedCategory = await _categoryRepository.Update(existingCategory);
        return CategoryMapper.ToDto(updatedCategory);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.Delete(id);
    }

    public async Task<bool> CategoryExistsAsync(int id)
    {
        var category = await _categoryRepository.GetById(id);
        return category != null;
    }

    public async Task<bool> CategoryNameExistsAsync(string name, int? excludeId = null)
    {
        var categories = await _categoryRepository.GetAll();
        return categories.Any(c => c.Name.ToLower() == name.ToLower() && 
                                 (!excludeId.HasValue || c.CategoryId != excludeId.Value));
    }
}
