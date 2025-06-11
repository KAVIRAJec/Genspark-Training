using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class SkillDTO
{
    public Guid? SkillId { get; set; }
    [Required(ErrorMessage = "Skill name is required.")]
    [MinLength(2, ErrorMessage = "Skill name must be at least 2 characters long.")]
    public string Name { get; set; } = string.Empty;
}