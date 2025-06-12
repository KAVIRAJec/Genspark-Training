using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateClientDTO
{
    public string? ProfileUrl { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string Username { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
    public string Password { get; set; } = string.Empty;
}
public class UpdateClientDTO
{
    public string? ProfileUrl { get; set; }
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string? Username { get; set; }
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
}
public class ClientResponseDTO
{
    public Guid Id { get; set; }
    public string? ProfileUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? Location { get; set; }
    public bool IsActive  { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<ProjectSummaryDTO> Projects { get; set; }
}