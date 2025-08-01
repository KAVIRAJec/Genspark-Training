using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class ColorMapper
{
    public static ColorDto ToDto(Color color)
    {
        return new ColorDto
        {
            ColorId = color.ColorId,
            Name = color.Name,
            HexCode = color.HexCode,
            IsActive = color.IsActive,
            CreatedDate = color.CreatedDate,
            ProductCount = color.Products?.Count(p => p.IsActive) ?? 0
        };
    }

    public static Color ToEntity(CreateColorDto createDto)
    {
        return new Color
        {
            Name = createDto.Name,
            HexCode = createDto.HexCode,
            IsActive = true
        };
    }

    public static void UpdateEntity(Color entity, UpdateColorDto updateDto)
    {
        entity.Name = updateDto.Name;
        entity.HexCode = updateDto.HexCode;
    }

    public static IEnumerable<ColorDto> ToDtoList(IEnumerable<Color> colors)
    {
        return colors.Select(ToDto);
    }
}
