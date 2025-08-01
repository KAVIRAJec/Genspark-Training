using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Image = product.Image,
            Price = product.Price,
            UserId = product.UserId,
            UserName = product.User?.UserName ?? "Unknown",
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? "Unknown",
            ColorId = product.ColorId,
            ColorName = product.Color?.Name ?? "Unknown",
            ColorHexCode = product.Color?.HexCode ?? "#000000",
            ModelId = product.ModelId,
            ModelName = product.Model?.Name ?? "Unknown",
            StorageId = product.StorageId,
            SellStartDate = product.SellStartDate,
            SellEndDate = product.SellEndDate,
            IsNew = product.IsNew,
            IsActive = product.IsActive,
            CreatedDate = product.CreatedDate,
        };
    }

    public static Product ToEntity(CreateProductDto createDto)
    {
        return new Product
        {
            ProductName = createDto.ProductName,
            Image = createDto.Image,
            Price = createDto.Price,
            UserId = createDto.UserId,
            CategoryId = createDto.CategoryId,
            ColorId = createDto.ColorId,
            ModelId = createDto.ModelId,
            StorageId = createDto.StorageId,
            SellStartDate = createDto.SellStartDate,
            SellEndDate = createDto.SellEndDate,
            IsNew = createDto.IsNew,
            IsActive = true
        };
    }

    public static void UpdateEntity(Product entity, UpdateProductDto updateDto)
    {
        entity.ProductName = updateDto.ProductName;
        entity.Image = updateDto.Image;
        entity.Price = updateDto.Price;
        entity.UserId = updateDto.UserId;
        entity.CategoryId = updateDto.CategoryId;
        entity.ColorId = updateDto.ColorId;
        entity.ModelId = updateDto.ModelId;
        entity.StorageId = updateDto.StorageId;
        entity.SellStartDate = updateDto.SellStartDate;
        entity.SellEndDate = updateDto.SellEndDate;
        entity.IsNew = updateDto.IsNew;
    }

    public static IEnumerable<ProductDto> ToDtoList(IEnumerable<Product> products)
    {
        return products.Select(ToDto);
    }
}
