using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProjectController : BaseApiController
{
    private readonly IClientProjectService _clientProjectService;
    private readonly IHubContext<NotificationHub> _hubContext;
    public ProjectController(IClientProjectService clientProjectService,
                             IHubContext<NotificationHub> hubContext)
    {
        _clientProjectService = clientProjectService;
        _hubContext = hubContext;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> PostProject([FromBody] CreateProjectDTO createProjectDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (Id != createProjectDTO.ClientId.ToString())
            return BadRequest("You are not authorized to create project using others' ID.");

        if (createProjectDTO == null) return BadRequest("CreateProjectDTO cannot be null");
        var result = await _clientProjectService.PostProject(createProjectDTO);

        await _hubContext.Clients.All.SendAsync("FreelancerNotification", $"A new project '{result.Title}' has been posted. Check it out!");
        return result != null ? Success(result) : BadRequest("Project creation failed");
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetProjectByProjectId(Guid projectId)
    {
        if (projectId == Guid.Empty) return BadRequest("Project ID is required.");
        var result = await _clientProjectService.GetProjectById(projectId);
        return result != null ? Success(result) : NotFound("Project not found");
    }

    [HttpGet("client/{clientId}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetProjectsByClientId(Guid clientId, [FromQuery] PaginationParams paginationParams)
    {
        if (clientId == Guid.Empty) return BadRequest("Client ID is required.");
        var result = await _clientProjectService.GetProjectsByClientId(clientId, paginationParams);
        return result != null ? Success(result) : NotFound("No projects found for this client.");
    }

    [HttpGet("freelancer/{freelancerId}")]
    public async Task<IActionResult> GetProjectsByFreelancerId(Guid freelancerId, [FromQuery] PaginationParams paginationParams)
    {
        if (freelancerId == Guid.Empty) return BadRequest("Freelancer ID is required.");
        var result = await _clientProjectService.GetProjectsByFreelancerId(freelancerId, paginationParams);
        return result != null ? Success(result) : NotFound("No projects found for this freelancer.");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllProjects([FromQuery] PaginationParams paginationParams)
    {
        var result = await _clientProjectService.GetAllProjectsPaged(paginationParams);
        return result != null ? Success(result) : NotFound("No projects found.");
    }

    [HttpPut("{projectId}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateProject([FromRoute] Guid projectId, [FromBody] UpdateProjectDTO updateProjectDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(projectId);
        if (project == null ||  project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to update this project.");

        if (updateProjectDTO == null) return BadRequest("UpdateProjectDTO cannot be null");
        var result = await _clientProjectService.UpdateProject(projectId, updateProjectDTO);

        if (result.FreelancerId != null && result.FreelancerId != Guid.Empty)
            await _hubContext.Clients.User(result.FreelancerId.ToString())
                           .SendAsync("FreelancerNotification", $"Your working project {result.Title} has been modified.");
        return result != null ? Success(result) : NotFound("Unable to update project");
    }

    [HttpDelete("{projectId}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteProject([FromRoute] Guid projectId)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(projectId);
        if (project == null || project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to delete this project.");

        if (projectId == Guid.Empty) return BadRequest("Project ID is required.");
        var result = await _clientProjectService.DeleteProject(projectId);

        if (result.FreelancerId != null && result.FreelancerId != Guid.Empty)
            await _hubContext.Clients.User(result.FreelancerId.ToString())
                           .SendAsync("FreelancerNotification", $"Your working project {result.Title} has been deleted.");
        return result != null ? Success(result) : NotFound("Unable to delete project");
    }
}