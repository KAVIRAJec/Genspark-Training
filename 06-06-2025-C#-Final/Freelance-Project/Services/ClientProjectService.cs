using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Services;

public class ClientProjectService : IClientProjectService
{
    private readonly IRepository<Guid, Project> _projectRepository;
    private readonly IRepository<Guid, Client> _clientRepository;
    private readonly IGetOrCreateSkills _getOrCreateSkills;
    protected readonly FreelanceDBContext _appContext;

    public ClientProjectService(IRepository<Guid, Project> projectRepository,
                                IRepository<Guid, Client> clientRepository,
                                IGetOrCreateSkills getOrCreateSkills,
                                FreelanceDBContext appContext)
    {
        _projectRepository = projectRepository;
        _clientRepository = clientRepository;
        _getOrCreateSkills = getOrCreateSkills;
        _appContext = appContext;
    }

    public async Task<ProjectResponseDTO> PostProject(CreateProjectDTO createProjectDTO)
    {
        var client = await _clientRepository.Get(createProjectDTO.ClientId);
        if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);
        var requiredSkills = new List<Skill>();
        if (createProjectDTO.RequiredSkills != null && createProjectDTO.RequiredSkills.Count() > 0)
            requiredSkills = await _getOrCreateSkills.GetOrCreateSkills(createProjectDTO.RequiredSkills);

        var project = ProjectMapper.CreateProjectFromCreateDTO(createProjectDTO, requiredSkills);
        var response = await _projectRepository.Add(project);
        if (response == null) throw new AppException("Project creation failed.", 500);
        return ProjectMapper.ToResponseDTO(response);
    }

    public async Task<ProjectResponseDTO> DeleteProject(Guid projectId)
    {
        var project = await _projectRepository.Get(projectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
        var response = await _projectRepository.Delete(projectId);
        if (response == null) throw new AppException("Project deletion failed.", 500);
        return ProjectMapper.ToResponseDTO(response);
    }

    public async Task<PagedResponse<ProjectResponseDTO>> GetAllProjectsPaged(PaginationParams paginationParams)
    {
        var query = _appContext.Projects
            .Where(p => p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ProjectMapper.ToResponseDTO(p));

        return await query.ToPagedResponse(paginationParams);
    }

    public async Task<ProjectResponseDTO> GetProjectById(Guid projectId)
    {
        if (projectId == Guid.Empty) throw new AppException("Project ID is required.", 400);
        var project = await _projectRepository.Get(projectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
        return ProjectMapper.ToResponseDTO(project);
    }

    public async Task<PagedResponse<ProjectResponseDTO>> GetProjectsByClientId(Guid clientId, PaginationParams paginationParams)
    {
        if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
        var client = await _clientRepository.Get(clientId);
        if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);

        var query = _appContext.Projects
            .Where(p => p.ClientId == clientId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ProjectMapper.ToResponseDTO(p));

        return await query.ToPagedResponse(paginationParams);
    }

    public async Task<ProjectResponseDTO> UpdateProject(Guid projectId, UpdateProjectDTO updateProjectDTO)
    {
        if (projectId == Guid.Empty) throw new AppException("Project ID is required.", 400);
        var project = await _projectRepository.Get(projectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
        //Pending, InProgress, Completed, Cancelled
        // if (updateProjectDTO.Status == null || (updateProjectDTO.Status != "Pending" && updateProjectDTO.Status != "Completed" && updateProjectDTO.Status != "Cancelled"))
        //     throw new AppException("Invalid project status.", 400);
        // if (updateProjectDTO.Status == "In Progress") 
        //     throw new AppException("Project status cannot be set to 'In Progress' directly. Use AcceptProposal to change status.", 400);
            
        var requiredSkills = new List<Skill>();
        if (updateProjectDTO.RequiredSkills != null && updateProjectDTO.RequiredSkills.Count() > 0)
            requiredSkills = await _getOrCreateSkills.GetOrCreateSkills(updateProjectDTO.RequiredSkills);

        project.Title = updateProjectDTO.Title ?? project.Title;
        project.Description = updateProjectDTO.Description ?? project.Description;
        project.Budget = updateProjectDTO.Budget ?? project.Budget;
        project.Duration = updateProjectDTO.Duration ?? project.Duration;
        // project.ClientId = updateProjectDTO.ClientId ?? project.ClientId;
        // project.FreelancerId = updateProjectDTO.FreelancerId ?? project.FreelancerId;
        // project.Status = updateProjectDTO.Status ?? project.Status;
        project.RequiredSkills = requiredSkills.Count > 0 ? requiredSkills : project.RequiredSkills;
        project.UpdatedAt = DateTime.UtcNow;

        var response = await _projectRepository.Update(projectId, project);
        if (response == null) throw new AppException("Project update failed.", 500);
        return ProjectMapper.ToResponseDTO(response);
    }
}