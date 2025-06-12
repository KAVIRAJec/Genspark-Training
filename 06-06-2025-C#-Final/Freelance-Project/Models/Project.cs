using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class Project
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Budget { get; set; }
    public TimeSpan? Duration { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? ClientId { get; set; }
    public Guid? FreelancerId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, In Progress, Completed, Cancelled

    public Client? Client { get; set; }
    public Freelancer? Freelancer { get; set; }
    public ICollection<Proposal>? Proposals { get; set; }
    public ICollection<Skill>? RequiredSkills { get; set; }
}