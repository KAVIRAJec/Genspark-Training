using System;
using System.Collections.Generic;
using System.Linq;
using DBTest.Models;
using DBTest.Interfaces;
using DBTest.Repositories;
using DBTest.Data;
using DBTest.DTOs;
using DBTest.Mapping;

namespace DBTest.Services
{
    public class ProductService : IProductService, IService<Product, int>
    {
        private readonly IRepository<Product, int> _repository;

        public ProductService(IRepository<Product, int> repository)
        {
            _repository = repository;
        }

        // IService<Product, int> implementation
        public Product Create(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            // Ensure CreatedAt is set
            if (product.CreatedAt == default(DateTime))
                product.CreatedAt = DateTime.Now;

            return _repository.Add(product);
        }

        public IEnumerable<Product> GetAll()
        {
            return _repository.GetAll();
        }

        public Product? GetById(int id)
        {
            return _repository.GetById(id);
        }

        public Product Update(Product product)
        {
            if (product.Id <= 0)
                throw new ArgumentException("Invalid product ID");

            if (string.IsNullOrEmpty(product.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (product.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            // Check if product exists
            var existingProduct = _repository.GetById(product.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {product.Id} not found");

            return _repository.Update(product);
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }
        
        // DTO methods
        public ProductDto CreateFromDto(CreateProductDto createDto)
        {
            if (string.IsNullOrEmpty(createDto.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (createDto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            // Convert DTO to entity
            var product = createDto.ToEntity();
            
            // Ensure CreatedAt is set
            if (product.CreatedAt == default(DateTime))
                product.CreatedAt = DateTime.Now;

            // Save to database
            var createdProduct = _repository.Add(product);

            // Convert back to DTO for response
            return createdProduct.ToDto();
        }

        public ProductDto UpdateFromDto(int id, UpdateProductDto updateDto)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid product ID");

            if (string.IsNullOrEmpty(updateDto.Name))
                throw new ArgumentException("Product name cannot be empty");

            if (updateDto.Price <= 0)
                throw new ArgumentException("Product price must be greater than zero");

            // Get existing product
            var existingProduct = _repository.GetById(id);
            if (existingProduct == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            // Update entity with DTO values
            updateDto.UpdateEntity(existingProduct);
            
            // Save changes
            var updatedProduct = _repository.Update(existingProduct);
            
            // Return updated product as DTO
            return updatedProduct.ToDto();
        }

        public IEnumerable<ProductDto> GetAllAsDto()
        {
            var products = _repository.GetAll();
            return products.ToDtos();
        }

        public ProductDto GetByIdAsDto(int id)
        {
            var product = _repository.GetById(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");
                
            return product.ToDto();
        }
    }
}
