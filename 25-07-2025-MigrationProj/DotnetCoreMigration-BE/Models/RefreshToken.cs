using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCoreMigration.Models;

[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    public int RefreshTokenId { get; set; }
    
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required]
    public int UserId { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public DateTime ExpirationDate { get; set; }
    
    public bool IsRevoked { get; set; } = false;
    
    public bool IsUsed { get; set; } = false;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
