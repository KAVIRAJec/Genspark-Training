namespace Freelance_Project.Models.DTO;

public class ClientSummaryDTO
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive  { get; set; }
}