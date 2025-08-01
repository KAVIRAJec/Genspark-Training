using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;

namespace DotnetCoreMigration.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product, int> _productRepository;

    public ProductService(IRepository<Product, int> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAll();
        return ProductMapper.ToDtoList(products);
    }

    public async Task<PaginatedResponse<ProductDto>> GetAllProductsPaginatedAsync(PaginationRequest request)
    {
        var paginatedProducts = await _productRepository.GetAllPaginated(request);
        var productDtos = ProductMapper.ToDtoList(paginatedProducts.Data);

        return new PaginatedResponse<ProductDto>(
            productDtos.ToList(),
            paginatedProducts.TotalCount,
            paginatedProducts.PageNumber,
            paginatedProducts.PageSize
        );
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetById(id);
        return product == null ? null : ProductMapper.ToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        // Check if product name already exists
        var products = await _productRepository.GetAll();
        if (products.Any(p => p.ProductName.ToLower() == createProductDto.ProductName.ToLower()))
        {
            throw new InvalidOperationException("Product name already exists.");
        }

        var product = ProductMapper.ToEntity(createProductDto);
        var createdProduct = await _productRepository.Create(product);
        return ProductMapper.ToDto(createdProduct);
    }

    public async Task<ProductDto> UpdateProductAsync(UpdateProductDto updateProductDto)
    {
        var existingProduct = await _productRepository.GetById(updateProductDto.ProductId);
        if (existingProduct == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        // Check if product name already exists (excluding current product)
        var products = await _productRepository.GetAll();
        if (products.Any(p => p.ProductId != updateProductDto.ProductId && 
                             p.ProductName.ToLower() == updateProductDto.ProductName.ToLower()))
        {
            throw new InvalidOperationException("Product name already exists.");
        }

        ProductMapper.UpdateEntity(existingProduct, updateProductDto);
        var updatedProduct = await _productRepository.Update(existingProduct);
        return ProductMapper.ToDto(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.Delete(id);
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        var product = await _productRepository.GetById(id);
        return product != null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetAll();
        var categoryProducts = products.Where(p => p.CategoryId == categoryId);
        return ProductMapper.ToDtoList(categoryProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByColorAsync(int colorId)
    {
        var products = await _productRepository.GetAll();
        var colorProducts = products.Where(p => p.ColorId == colorId);
        return ProductMapper.ToDtoList(colorProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByModelAsync(int modelId)
    {
        var products = await _productRepository.GetAll();
        var modelProducts = products.Where(p => p.ModelId == modelId);
        return ProductMapper.ToDtoList(modelProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByUserAsync(int userId)
    {
        var products = await _productRepository.GetAll();
        var userProducts = products.Where(p => p.UserId == userId);
        return ProductMapper.ToDtoList(userProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetNewProductsAsync()
    {
        var products = await _productRepository.GetAll();
        var newProducts = products.Where(p => p.IsNew == 1);
        return ProductMapper.ToDtoList(newProducts);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetAll();
        var activeProducts = products.Where(p => p.IsActive);
        return ProductMapper.ToDtoList(activeProducts);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.GetAll();
        var searchResults = products.Where(p => 
            p.ProductName.ToLower().Contains(searchTerm.ToLower()) ||
            (p.Category != null && p.Category.Name.ToLower().Contains(searchTerm.ToLower())) ||
            (p.Color != null && p.Color.Name.ToLower().Contains(searchTerm.ToLower())) ||
            (p.Model != null && p.Model.Name.ToLower().Contains(searchTerm.ToLower()))
        );
        return ProductMapper.ToDtoList(searchResults);
    }
}
