using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateProposalDTO
{
    public string Description { get; set; }
    public decimal ProposedAmount { get; set; }
    public TimeSpan? ProposedDuration { get; set; }
    [Required(ErrorMessage = "Freelancer ID is required.")]
    public Guid FreelancerId { get; set; }
    [Required(ErrorMessage = "Project ID is required.")]
    public Guid ProjectId { get; set; }
}
public class UpdateProposalDTO
{
    public string? Description { get; set; }
    public decimal? ProposedAmount { get; set; }
    public TimeSpan? ProposedDuration { get; set; }
}
public class ProposalResponseDTO
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "Proposed amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Proposed amount must be greater than zero.")]
    public decimal ProposedAmount { get; set; }
    [Required(ErrorMessage = "Proposed duration is required.")]
    public TimeSpan ProposedDuration { get; set; }
    public bool IsActive { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsRejected { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public FreelancerSummaryDTO Freelancer { get; set; }
    public ProjectSummaryDTO Project { get; set; }
}