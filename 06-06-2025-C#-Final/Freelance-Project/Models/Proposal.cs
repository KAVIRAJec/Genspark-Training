using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models;

public class Proposal
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Description { get; set; }
    public decimal ProposedAmount { get; set; }
    public TimeSpan? ProposedDuration { get; set; }
    public bool IsActive { get; set; } = true;
    public bool? IsAccepted { get; set; }
    public bool? IsRejected { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid FreelancerId { get; set; }
    public Guid ProjectId { get; set; }

    public Freelancer? Freelancer { get; set; }
    public Project? Project { get; set; }
}
