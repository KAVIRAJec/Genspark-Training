using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Misc;

public static class ClientMapper
{
    public static ClientResponseDTO ToResponseDTO(Client client)
    {
        return new ClientResponseDTO
        {
            Id = client.Id,
            ProfileUrl = client.ProfileUrl,
            Username = client.Username,
            Email = client.Email,
            CompanyName = client.CompanyName,
            Location = client.Location,
            IsActive = client.IsActive,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt,
            DeletedAt = client.DeletedAt,
            Projects = client.Projects?.Select(p => new ProjectSummaryDTO
            {
                Id = p.Id,
                Title = p.Title,
                Status = p.Status,
                IsActive = p.IsActive,
                FreelancerId = p.FreelancerId
            }).Where(p => p.IsActive == true).ToList() ?? new List<ProjectSummaryDTO>()
        };
    }
    public static Client CreateClientFromCreateDTO(CreateClientDTO createClientDTO)
    {
        if (createClientDTO == null) throw new AppException("Client DTO cannot be null.", 400);

        return new Client
        {
            ProfileUrl = createClientDTO.ProfileUrl ?? null,
            Username = createClientDTO.Username,
            Email = createClientDTO.Email,
            CompanyName = createClientDTO.CompanyName,
            Location = createClientDTO.Location ?? null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}