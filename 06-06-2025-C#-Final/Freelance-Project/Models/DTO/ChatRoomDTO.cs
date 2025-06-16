using System.ComponentModel.DataAnnotations;

namespace Freelance_Project.Models.DTO;

public class CreateChatRoomDTO
{
    [Required(ErrorMessage = "Name is required.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Client ID is required.")]
    public Guid ClientId { get; set; }
    [Required(ErrorMessage = "Freelancer ID is required.")]
    public Guid FreelancerId { get; set; }
    [Required(ErrorMessage = "Project ID is required.")]
    public Guid ProjectId { get; set; }
}
public class UpdateChatRoomDTO
{
}
public class ChatRoomResponseDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid ClientId { get; set; }
    public Guid FreelancerId { get; set; }
    public Guid ProjectId { get; set; }

    public string ClientName { get; set; }
    public string FreelancerName { get; set; }
    public ICollection<ChatMessageResponseDTO>? Messages { get; set; }
}