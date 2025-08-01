using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class OrderDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public string? PaypalPaymentId { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; } = new List<OrderDetailDto>();
}

public class OrderDetailDto
{
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public bool IsActive { get; set; }
}

public class CreateOrderDto
{
    [Required]
    public int UserId { get; set; }
    
    [MaxLength(500)]
    public string? ShippingAddress { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }

    [MaxLength(100)]
    public string? PaypalPaymentId { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one order detail is required")]
    public List<CreateOrderDetailDto> OrderDetails { get; set; } = new List<CreateOrderDetailDto>();
}

public class CreateOrderDetailDto
{
    [Required]
    public int ProductId { get; set; }
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderDto
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? ShippingAddress { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class UpdateOrderStatusDto
{
    [Required]
    public int OrderId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
