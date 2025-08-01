using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCoreMigration.Models;

[Table("Products")]
public class Product
{
    [Key]
    public int ProductId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Image { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    
    public int? UserId { get; set; }
    
    public int? CategoryId { get; set; }
    
    public int? ColorId { get; set; }
    
    public int? ModelId { get; set; }
    
    public int? StorageId { get; set; }
    
    public DateTime? SellStartDate { get; set; }
    
    public DateTime? SellEndDate { get; set; }
    
    public int? IsNew { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
    
    [ForeignKey("ColorId")]
    public virtual Color? Color { get; set; }

    [ForeignKey("ModelId")]
    public virtual Model? Model { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
    
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
