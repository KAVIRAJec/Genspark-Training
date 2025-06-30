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
public class ProjectProposalController : BaseApiController
{

    private readonly IProjectProposalService _projectProposalService;
    private readonly IClientProjectService _clientProjectService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProjectProposalController(IProjectProposalService projectProposalService,
                                     IClientProjectService clientProjectService,
                                     IHubContext<NotificationHub> hubContext)
    {
        _projectProposalService = projectProposalService;
        _clientProjectService = clientProjectService;
        _hubContext = hubContext;
    }

    [HttpGet("ProjectId/{projectId}")]
    public async Task<IActionResult> GetProposalsByProjectId(Guid projectId, [FromQuery] PaginationParams paginationParams)
    {
        if (projectId == Guid.Empty) return BadRequest("Project ID cannot be empty.");

        var proposals = await _projectProposalService.GetProposalsByProjectId(projectId, paginationParams);
        return proposals != null ? Success(proposals) : NotFound("No proposals found for the given project ID.");
    }

    [HttpPost("Accept")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> AcceptProposal([FromBody] ProposalRequestDTO requestDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(requestDTO.ProjectId);
        if (project == null || project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to accept proposals for other clients' projects.");

        if (requestDTO == null || requestDTO.ProposalId == Guid.Empty || requestDTO.ProjectId == Guid.Empty)
            return BadRequest("Invalid proposal or project ID.");

        var projectResponse = await _projectProposalService.AcceptProposal(requestDTO.ProposalId, requestDTO.ProjectId);

        await _hubContext.Clients.User(projectResponse.FreelancerId.ToString())
                           .SendAsync("FreelancerNotification", $"Your proposal for project {projectResponse.Title} has been accepted.");
        return projectResponse != null ? Success(projectResponse) : NotFound("Project or proposal not found.");
    }

    [HttpPost("Cancel")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CancelProject([FromBody] CompleteProjectRequestDTO request)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(request.ProjectId);
        if (project == null || project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to cancel proposals for other clients' projects.");

        if (request.ProjectId == Guid.Empty) return BadRequest("Project ID cannot be empty.");

        var projectResponse = await _projectProposalService.CancelProject(request.ProjectId);
        await _hubContext.Clients.User(projectResponse.FreelancerId.ToString())
                          .SendAsync("FreelancerNotification", $"Your proposal for project {projectResponse.Title} has been cancelled.");
        return projectResponse != null ? Success(projectResponse) : NotFound("Project not found.");
    }

    [HttpPost("Reject")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> RejectProposal([FromBody] ProposalRequestDTO requestDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(requestDTO.ProjectId);
        if (project == null || project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to reject proposals for other clients' projects.");

        if (requestDTO == null || requestDTO.ProposalId == Guid.Empty || requestDTO.ProjectId == Guid.Empty)
            return BadRequest("Invalid proposal or project ID.");

        var proposalResponse = await _projectProposalService.RejectProposal(requestDTO.ProposalId, requestDTO.ProjectId);

        await _hubContext.Clients.User(proposalResponse.Freelancer.Id.ToString())
                          .SendAsync("FreelancerNotification", $"Your proposal for project {proposalResponse.Project.Title} has been rejected.");
        return proposalResponse != null ? Success(proposalResponse) : NotFound("Project or proposal not found.");
    }

    public class CompleteProjectRequestDTO
    {
        public Guid ProjectId { get; set; }
    }
    [HttpPost("Complete")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> CompleteProject([FromBody] CompleteProjectRequestDTO request)
    {
        if (request == null || request.ProjectId == Guid.Empty)
            return BadRequest("Project ID cannot be empty.");

        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var project = await _clientProjectService.GetProjectById(request.ProjectId);
        if (project == null || project.ClientId.ToString() != Id)
            return BadRequest("You are not authorized to complete projects for other clients' projects.");

        var projectResponse = await _projectProposalService.CompleteProject(request.ProjectId);
        // Freelancer Notification
        await _hubContext.Clients.User(projectResponse.FreelancerId.ToString())
                          .SendAsync("FreelancerNotification", $"Your proposal for project {projectResponse.Title} has been completed.");

        // Client Notification
        await _hubContext.Clients.User(projectResponse.ClientId.ToString())
                           .SendAsync("ClientNotification", $"Your project {projectResponse.Title} has been completed.");
        return projectResponse != null ? Success(projectResponse) : NotFound("Project not found.");
    }
}