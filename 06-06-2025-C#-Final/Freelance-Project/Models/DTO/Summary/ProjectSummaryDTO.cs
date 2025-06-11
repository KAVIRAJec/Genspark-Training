namespace Freelance_Project.Models.DTO;

public class ProjectSummaryDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; }
    public bool IsActive { get; set; }
    public Guid? FreelancerId { get; set; }
    public Guid? ClientId { get; set; }
}