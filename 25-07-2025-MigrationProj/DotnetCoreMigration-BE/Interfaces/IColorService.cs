using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IColorService
{
    Task<IEnumerable<ColorDto>> GetAllColorsAsync();
    Task<PaginatedResponse<ColorDto>> GetAllColorsPaginatedAsync(PaginationRequest request);
    Task<ColorDto?> GetColorByIdAsync(int id);
    Task<ColorDto> CreateColorAsync(CreateColorDto createColorDto);
    Task<ColorDto> UpdateColorAsync(UpdateColorDto updateColorDto);
    Task<bool> DeleteColorAsync(int id);
    Task<bool> ColorExistsAsync(int id);
    Task<bool> ColorNameExistsAsync(string name, int? excludeId = null);
}
