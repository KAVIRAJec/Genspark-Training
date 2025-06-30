using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IClientProjectService
{
    public Task<ProjectResponseDTO> PostProject(CreateProjectDTO createProjectDTO);
    public Task<PagedResponse<ProjectResponseDTO>> GetAllProjectsPaged(PaginationParams paginationParams);
    public Task<ProjectResponseDTO> GetProjectById(Guid projectId);
    public Task<PagedResponse<ProjectResponseDTO>> GetProjectsByClientId(Guid clientId, PaginationParams paginationParams);
    public Task<PagedResponse<ProjectResponseDTO>> GetProjectsByFreelancerId(Guid freelancerId, PaginationParams paginationParams);
    public Task<ProjectResponseDTO> UpdateProject(Guid projectId, UpdateProjectDTO updateProjectDTO);
    public Task<ProjectResponseDTO> DeleteProject(Guid projectId);
}