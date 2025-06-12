using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateFreelancerDTO
{
    public string? ProfileUrl { get; set; }
    [Required]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    public string Username { get; set; } = string.Empty;
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    [Required]
    [Range(0, 40, ErrorMessage = "Experience years must be between 0 and 40.")]
    public int ExperienceYears { get; set; }
    [Required]
    [Range(0, 10000, ErrorMessage = "Hourly rate must be between 0 and 10000.")]
    public decimal HourlyRate { get; set; }
    public string? Location { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one digit.")]
    public string Password { get; set; } = string.Empty;
    public ICollection<SkillDTO>? Skills { get; set; }
}
public class UpdateFreelancerDTO
{
    public string? ProfileUrl { get; set; }
    public string? Username { get; set; }
    public int? ExperienceYears { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? Location { get; set; }
    public ICollection<SkillDTO>? Skills { get; set; }
}
public class FreelancerResponseDTO
{
    public Guid Id { get; set; }
    public string? ProfileUrl { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<SkillDTO>? Skills { get; set; }
}