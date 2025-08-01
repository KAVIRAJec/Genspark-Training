using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            IsActive = category.IsActive,
            CreatedDate = category.CreatedDate,
            ProductCount = category.Products?.Count(p => p.IsActive) ?? 0
        };
    }

    public static Category ToEntity(CreateCategoryDto createDto)
    {
        return new Category
        {
            Name = createDto.Name,
            IsActive = true
        };
    }

    public static void UpdateEntity(Category entity, UpdateCategoryDto updateDto)
    {
        entity.Name = updateDto.Name;
    }

    public static IEnumerable<CategoryDto> ToDtoList(IEnumerable<Category> categories)
    {
        return categories.Select(ToDto);
    }
}
