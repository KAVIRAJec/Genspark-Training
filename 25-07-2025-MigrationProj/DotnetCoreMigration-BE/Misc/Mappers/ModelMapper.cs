using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class ModelMapper
{
    public static ModelDto ToDto(Model model)
    {
        return new ModelDto
        {
            ModelId = model.ModelId,
            Name = model.Name,
            CreatedDate = model.CreatedDate,
            IsActive = model.IsActive,
            ProductCount = model.Products?.Count(p => p.IsActive) ?? 0
        };
    }

    public static Model ToEntity(CreateModelDto createDto)
    {
        return new Model
        {
            Name = createDto.Name,
            IsActive = true
        };
    }

    public static void UpdateEntity(Model entity, UpdateModelDto updateDto)
    {
        entity.Name = updateDto.Name;
    }

    public static IEnumerable<ModelDto> ToDtoList(IEnumerable<Model> models)
    {
        return models.Select(ToDto);
    }
}
