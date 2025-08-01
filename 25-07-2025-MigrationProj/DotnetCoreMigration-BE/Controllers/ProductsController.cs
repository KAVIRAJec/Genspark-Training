using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAllProducts()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products", ex.Message));
        }
    }

    /// <summary>
    /// Get all products with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<ProductDto>>>> GetAllProductsPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedProducts = await _productService.GetAllProductsPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<ProductDto>>.SuccessResponse(paginatedProducts, "Paginated products retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<ProductDto>>.ErrorResponse("An error occurred while retrieving products", ex.Message));
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(int id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found", $"No product found with ID {id}"));
            }
            return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while retrieving the product", ex.Message));
        }
    }

    /// <summary>
    /// Get products by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByCategory(int categoryId)
    {
        try
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products by category retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products by category", ex.Message));
        }
    }

    /// <summary>
    /// Get products by color
    /// </summary>
    [HttpGet("color/{colorId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByColor(int colorId)
    {
        try
        {
            var products = await _productService.GetProductsByColorAsync(colorId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products by color retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products by color", ex.Message));
        }
    }

    /// <summary>
    /// Get products by model
    /// </summary>
    [HttpGet("model/{modelId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByModel(int modelId)
    {
        try
        {
            var products = await _productService.GetProductsByModelAsync(modelId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products by model retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products by model", ex.Message));
        }
    }

    /// <summary>
    /// Get products by user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetProductsByUser(int userId)
    {
        try
        {
            var products = await _productService.GetProductsByUserAsync(userId);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Products by user retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving products by user", ex.Message));
        }
    }

    /// <summary>
    /// Get new products
    /// </summary>
    [HttpGet("new")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetNewProducts()
    {
        try
        {
            var products = await _productService.GetNewProductsAsync();
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "New products retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving new products", ex.Message));
        }
    }

    /// <summary>
    /// Get active products
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetActiveProducts()
    {
        try
        {
            var products = await _productService.GetActiveProductsAsync();
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Active products retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while retrieving active products", ex.Message));
        }
    }

    /// <summary>
    /// Search products
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> SearchProducts([FromQuery] string searchTerm)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("Search term is required", "Please provide a search term"));
            }

            var products = await _productService.SearchProductsAsync(searchTerm);
            return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(products, "Product search completed successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<ProductDto>>.ErrorResponse("An error occurred while searching products", ex.Message));
        }
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var product = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, ApiResponse<ProductDto>.SuccessResponse(product, "Product created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Product creation failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while creating the product", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            if (id != updateProductDto.ProductId)
            {
                return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var exists = await _productService.ProductExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found", $"No product found with ID {id}"));
            }

            var product = await _productService.UpdateProductAsync(updateProductDto);
            return Ok(ApiResponse<ProductDto>.SuccessResponse(product, "Product updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<ProductDto>.ErrorResponse("Product update failed", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ProductDto>.ErrorResponse("An error occurred while updating the product", ex.Message));
        }
    }

    /// <summary>
    /// Delete a product (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteProduct(int id)
    {
        try
        {
            var exists = await _productService.ProductExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Product not found", $"No product found with ID {id}"));
            }

            var result = await _productService.DeleteProductAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Product deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the product", ex.Message));
        }
    }
}
