using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using DBTest.Models;
using DBTest.Services;
using DBTest.DTOs;

namespace DBTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<ProductDto>> GetProducts()
        {
            try
            {
                return Ok(_productService.GetAllAsDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<ProductDto> GetProduct(int id)
        {
            try
            {
                var product = _productService.GetByIdAsDto(id);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/Products
        [HttpPost]
        public ActionResult<ProductDto> CreateProduct(CreateProductDto createDto)
        {
            try
            {
                var createdProduct = _productService.CreateFromDto(createDto);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Internal server error: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Inner exception: {ex.InnerException.Message}";
                }
                return StatusCode(500, new { Error = errorMessage });
            }
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, UpdateProductDto updateDto)
        {
            try
            {
                var updatedProduct = _productService.UpdateFromDto(id, updateDto);
                return Ok(updatedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var result = _productService.Delete(id);
                if (result)
                    return NoContent();
                else
                    return NotFound($"Product with ID {id} not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
