namespace Freelance_Project.Models.DTO;

public class ProposalSummaryDTO
{
    public Guid Id { get; set; }
    public Guid FreelancerId { get; set; }
    public decimal ProposedAmount { get; set; }
    public TimeSpan? ProposedDuration { get; set; }
    public bool IsActive { get; set; }
}