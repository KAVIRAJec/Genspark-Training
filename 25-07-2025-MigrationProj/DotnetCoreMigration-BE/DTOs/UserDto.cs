using System.ComponentModel.DataAnnotations;

namespace DotnetCoreMigration.DTOs;

public class UserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
    public string Role { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public int ProductCount { get; set; }
    public int NewsCount { get; set; }
}

public class CreateUserDto
{
    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
}

public class UpdateUserDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(255)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(50)]
    public string? Role { get; set; }
}

public class UpdateUserProfileDto
{
    [MaxLength(255)]
    public string? FirstName { get; set; }

    [MaxLength(255)]
    public string? LastName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}
