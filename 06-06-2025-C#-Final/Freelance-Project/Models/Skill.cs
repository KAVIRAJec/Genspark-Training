using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class Skill
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<Freelancer>? Freelancers { get; set; }
    public ICollection<Project>? Projects { get; set; }
}
