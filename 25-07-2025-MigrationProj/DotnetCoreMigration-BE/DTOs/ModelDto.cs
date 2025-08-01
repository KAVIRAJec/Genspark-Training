using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class ModelDto
{
    public int ModelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
}

public class CreateModelDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}

public class UpdateModelDto
{
    [Required]
    public int ModelId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}
