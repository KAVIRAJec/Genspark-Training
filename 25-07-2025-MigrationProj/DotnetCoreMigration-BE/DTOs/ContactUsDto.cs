using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class ContactUsDto
{
    public int ContactId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsRead { get; set; }
    public bool IsActive { get; set; }
    public string? Response { get; set; }
    public DateTime? ResponseDate { get; set; }
}

public class CreateContactUsDto
{
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
}

public class UpdateContactUsDto
{
    [Required]
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
    
    public bool IsRead { get; set; }
    public string? Response { get; set; }
    public DateTime? ResponseDate { get; set; }
}
