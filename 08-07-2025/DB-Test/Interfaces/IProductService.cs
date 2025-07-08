using System.Collections.Generic;
using DBTest.Models;
using DBTest.DTOs;
using DBTest.Interfaces;

namespace DBTest.Services
{
    public interface IProductService : IService<Product, int>
    {
        // Additional DTO methods
        ProductDto CreateFromDto(CreateProductDto createDto);
        ProductDto UpdateFromDto(int id, UpdateProductDto updateDto);
        IEnumerable<ProductDto> GetAllAsDto();
        ProductDto GetByIdAsDto(int id);
    }
}
