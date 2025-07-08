using DBTest.DTOs;
using DBTest.Models;
using System.Collections.Generic;
using System.Linq;

namespace DBTest.Mapping
{
    public static class ProductMapper
    {
        // Map from CreateProductDto to Product
        public static Product ToEntity(this CreateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity
                // CreatedAt is set by default value or database
            };
        }

        // Map from UpdateProductDto to Product (requires existing entity for Id)
        public static void UpdateEntity(this UpdateProductDto dto, Product entity)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Price = dto.Price;
            entity.StockQuantity = dto.StockQuantity;
            // We don't update Id or CreatedAt
        }

        // Map from Product to ProductDto
        public static ProductDto ToDto(this Product entity)
        {
            return new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity,
                CreatedAt = entity.CreatedAt
            };
        }

        // Map from IEnumerable<Product> to IEnumerable<ProductDto>
        public static IEnumerable<ProductDto> ToDtos(this IEnumerable<Product> entities)
        {
            return entities.Select(e => e.ToDto());
        }
    }
}
