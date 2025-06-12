using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateProjectDTO
{
    [Required(ErrorMessage = "Title is required.")]
    [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required(ErrorMessage = "Budget is required.")]
    public decimal Budget { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid ClientId { get; set; }
    public ICollection<SkillDTO>? RequiredSkills { get; set; }
}
public class UpdateProjectDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Budget { get; set; }
    public TimeSpan? Duration { get; set; }
    // public Guid? ClientId { get; set; }
    // public Guid? FreelancerId { get; set; }
    // public string? Status { get; set; }
    public ICollection<SkillDTO>? RequiredSkills { get; set; }
}
public class ProjectResponseDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Budget { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? FreelancerId { get; set; }
    public ICollection<ProposalSummaryDTO>? Proposals { get; set; }
    public ICollection<SkillDTO>? RequiredSkills { get; set; }
}