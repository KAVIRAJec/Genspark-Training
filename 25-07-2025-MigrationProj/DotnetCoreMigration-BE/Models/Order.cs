using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCoreMigration.Models;

[Table("Orders")]
public class Order
{
    [Key]
    public int OrderId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = "Pending"; // Pending, Delivered, Cancelled

    [MaxLength(100)]
    public string? PaypalPaymentId { get; set; } // Store PayPal payment id
    
    [MaxLength(500)]
    public string? ShippingAddress { get; set; }
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
    
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
