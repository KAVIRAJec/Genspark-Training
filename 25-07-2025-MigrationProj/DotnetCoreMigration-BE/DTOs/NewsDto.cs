using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class NewsDto
{
    public int NewsId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? Image { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsPublished { get; set; }
    public bool IsActive { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}

public class CreateNewsDto
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Summary { get; set; }
    
    [MaxLength(500)]
    public string? Image { get; set; }
    
    public bool IsPublished { get; set; } = true;
    
    [Required]
    public int AuthorId { get; set; }
}

public class UpdateNewsDto
{
    [Required]
    public int NewsId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Summary { get; set; }
    
    [MaxLength(500)]
    public string? Image { get; set; }
    
    public bool IsPublished { get; set; } = true;
    
    [Required]
    public int AuthorId { get; set; }
}
