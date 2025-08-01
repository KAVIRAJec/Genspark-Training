using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Image { get; set; }
    public decimal? Price { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? ColorId { get; set; }
    public string? ColorName { get; set; }
    public string? ColorHexCode { get; set; }
    public int? ModelId { get; set; }
    public string? ModelName { get; set; }
    public int? StorageId { get; set; }
    public DateTime? SellStartDate { get; set; }
    public DateTime? SellEndDate { get; set; }
    public int? IsNew { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateProductDto
{
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Image { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
    
    public int? UserId { get; set; }
    
    public int? CategoryId { get; set; }
    
    public int? ColorId { get; set; }
    
    public int? ModelId { get; set; }
    
    public int? StorageId { get; set; }
    
    public DateTime? SellStartDate { get; set; }
    
    public DateTime? SellEndDate { get; set; }
    
    [Range(0, 1, ErrorMessage = "IsNew must be 0 or 1")]
    public int? IsNew { get; set; }
}

public class UpdateProductDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Image { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal? Price { get; set; }
    
    public int? UserId { get; set; }
    
    public int? CategoryId { get; set; }
    
    public int? ColorId { get; set; }
    
    public int? ModelId { get; set; }
    
    public int? StorageId { get; set; }
    
    public DateTime? SellStartDate { get; set; }
    
    public DateTime? SellEndDate { get; set; }
    
    [Range(0, 1, ErrorMessage = "IsNew must be 0 or 1")]
    public int? IsNew { get; set; }
}
