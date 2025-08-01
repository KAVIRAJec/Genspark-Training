using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class RefreshTokenDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
