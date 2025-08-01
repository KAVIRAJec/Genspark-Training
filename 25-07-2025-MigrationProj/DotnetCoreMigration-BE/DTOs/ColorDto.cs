using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class ColorDto
{
    public int ColorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HexCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CreateColorDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Hex code must be in format #RRGGBB")]
    public string HexCode { get; set; } = string.Empty;
}

public class UpdateColorDto
{
    [Required]
    public int ColorId { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Hex code must be in format #RRGGBB")]
    public string HexCode { get; set; } = string.Empty;
}
