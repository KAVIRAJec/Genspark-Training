using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<PaginatedResponse<ProductDto>> GetAllProductsPaginatedAsync(PaginationRequest request);
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
    Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductDto>> GetProductsByColorAsync(int colorId);
    Task<IEnumerable<ProductDto>> GetProductsByModelAsync(int modelId);
    Task<IEnumerable<ProductDto>> GetProductsByUserAsync(int userId);
    Task<IEnumerable<ProductDto>> GetNewProductsAsync();
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync();
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
}
