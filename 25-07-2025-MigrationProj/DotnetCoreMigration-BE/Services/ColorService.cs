using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;
using Microsoft.EntityFrameworkCore;

namespace DotnetCoreMigration.Services;

public class ColorService : IColorService
{
    private readonly IRepository<Color, int> _colorRepository;

    public ColorService(IRepository<Color, int> colorRepository)
    {
        _colorRepository = colorRepository;
    }

    public async Task<IEnumerable<ColorDto>> GetAllColorsAsync()
    {
        var colors = await _colorRepository.GetAll();
        return ColorMapper.ToDtoList(colors);
    }

    public async Task<PaginatedResponse<ColorDto>> GetAllColorsPaginatedAsync(PaginationRequest request)
    {
        var paginatedColors = await _colorRepository.GetAllPaginated(request);
        var colorDtos = ColorMapper.ToDtoList(paginatedColors.Data);

        return new PaginatedResponse<ColorDto>(
            colorDtos.ToList(),
            paginatedColors.TotalCount,
            paginatedColors.PageNumber,
            paginatedColors.PageSize
        );
    }

    public async Task<ColorDto?> GetColorByIdAsync(int id)
    {
        var color = await _colorRepository.GetById(id);
        return color == null ? null : ColorMapper.ToDto(color);
    }

    public async Task<ColorDto> CreateColorAsync(CreateColorDto createColorDto)
    {
        // Check if color name already exists
        var colors = await _colorRepository.GetAll();
        if (colors.Any(c => c.Name.ToLower() == createColorDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Color name already exists.");
        }

        var color = ColorMapper.ToEntity(createColorDto);
        var createdColor = await _colorRepository.Create(color);
        return ColorMapper.ToDto(createdColor);
    }

    public async Task<ColorDto> UpdateColorAsync(UpdateColorDto updateColorDto)
    {
        var existingColor = await _colorRepository.GetById(updateColorDto.ColorId);
        if (existingColor == null)
        {
            throw new InvalidOperationException("Color not found.");
        }

        // Check if color name already exists (excluding current color)
        var colors = await _colorRepository.GetAll();
        if (colors.Any(c => c.ColorId != updateColorDto.ColorId && 
                           c.Name.ToLower() == updateColorDto.Name.ToLower()))
        {
            throw new InvalidOperationException("Color name already exists.");
        }

        ColorMapper.UpdateEntity(existingColor, updateColorDto);
        var updatedColor = await _colorRepository.Update(existingColor);
        return ColorMapper.ToDto(updatedColor);
    }

    public async Task<bool> DeleteColorAsync(int id)
    {
        return await _colorRepository.Delete(id);
    }

    public async Task<bool> ColorExistsAsync(int id)
    {
        var color = await _colorRepository.GetById(id);
        return color != null;
    }

    public async Task<bool> ColorNameExistsAsync(string name, int? excludeId = null)
    {
        var colors = await _colorRepository.GetAll();
        return colors.Any(c => c.Name.ToLower() == name.ToLower() && 
                              (!excludeId.HasValue || c.ColorId != excludeId.Value));
    }
}
