using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class CategoryDto
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateCategoryDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryDto
{
    [Required]
    public int CategoryId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}
