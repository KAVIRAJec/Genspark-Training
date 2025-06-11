using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class RefreshToken
{
    [Key]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Token is required.")]
    public string Token { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Expires is required.")]
    public DateTime Expires { get; set; }
}