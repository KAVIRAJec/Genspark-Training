using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetCoreMigration.Models;

[Table("ContactUs")]
public class ContactUs
{
    [Key]
    public int ContactId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(255)]
    public string? Subject { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    public bool IsRead { get; set; } = false;
    
    public string? Response { get; set; }
    
    public DateTime? ResponseDate { get; set; }
    
    public bool IsActive { get; set; } = true;
}
