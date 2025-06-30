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
public class ProposalController : BaseApiController
{
    private readonly IFreelancerProposalService _freelancerProposalService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProposalController(IFreelancerProposalService freelancerProposalService,
                               IHubContext<NotificationHub> hubContext)
    {
        _freelancerProposalService = freelancerProposalService;
        _hubContext = hubContext;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> CreateProposal([FromBody] CreateProposalDTO createProposalDTO)
    {
        if (createProposalDTO == null) return BadRequest("CreateProposalDTO cannot be null");

        var id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (id != createProposalDTO.FreelancerId.ToString())
            return BadRequest("You are not authorized to create a proposal using others' ID.");
        
        var result = await _freelancerProposalService.CreateProposal(createProposalDTO);

        await _hubContext.Clients.User(result.Project.ClientId.ToString())
                           .SendAsync("ClientNotification", "New proposal received");
        return result != null ? Success(result) : BadRequest("Proposal creation failed");
    }

    [HttpGet("{proposalId}")]
    public async Task<IActionResult> GetProposalByProposalId(Guid proposalId)
    {
        if (proposalId == Guid.Empty) return BadRequest("Proposal ID is required.");

        var result = await _freelancerProposalService.GetProposalById(proposalId);
        return result != null ? Success(result) : NotFound("Proposal not found");
    }

    [HttpGet("freelancer/{freelancerId}")]
    public async Task<IActionResult> GetProposalsByFreelancerId(Guid freelancerId, [FromQuery] PaginationParams paginationParams)
    {
        if (freelancerId == Guid.Empty) return BadRequest("Freelancer ID is required.");

        var result = await _freelancerProposalService.GetProposalsByFreelancerId(freelancerId, paginationParams);
        return result != null ? Success(result) : NotFound("No proposals found for this freelancer.");
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetProposalsByClientId(Guid clientId, [FromQuery] PaginationParams paginationParams)
    {
        if (clientId == Guid.Empty) return BadRequest("Client ID is required.");

        var result = await _freelancerProposalService.GetProposalsByClientId(clientId, paginationParams);
        return result != null ? Success(result) : NotFound("No proposals found for this client.");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllProposals([FromQuery] PaginationParams paginationParams)
    {
        var result = await _freelancerProposalService.GetAllProposalsPaged(paginationParams);
        return result != null ? Success(result) : NotFound("No proposals found.");
    }
    
    [HttpPut("{proposalId}")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> UpdateProposal([FromRoute] Guid proposalId, [FromBody] UpdateProposalDTO updateProposalDTO)
    {
        if (proposalId == Guid.Empty) return BadRequest("Proposal ID is required.");
        if (updateProposalDTO == null) return BadRequest("UpdateProposalDTO cannot be null.");

        var id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var proposal = await _freelancerProposalService.GetProposalById(proposalId);
        if (proposal == null || proposal.Freelancer.Id.ToString() != id)
            return BadRequest("You are not authorized to update this proposal.");

        var result = await _freelancerProposalService.UpdateProposal(proposalId, updateProposalDTO);

        if(result.Project.FreelancerId != null && result.Project.FreelancerId != Guid.Empty)
            await _hubContext.Clients.User(result.Project.ClientId.ToString())
                           .SendAsync("ClientNotification", "Proposal linked with your project has been updated.");
        return result != null ? Success(result) : NotFound("Proposal not found or update failed.");
    }

    [HttpDelete("{proposalId}")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> DeleteProposal([FromRoute] Guid proposalId)
    {
        var id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var proposal = await _freelancerProposalService.GetProposalById(proposalId);
        if (proposal == null || proposal.Freelancer.Id.ToString() != id)
            return BadRequest("You are not authorized to delete this proposal.");

        if (proposalId == Guid.Empty) return BadRequest("Proposal ID is required.");

        var result = await _freelancerProposalService.DeleteProposal(proposalId);

        if(result.Project.FreelancerId != null && result.Project.FreelancerId != Guid.Empty)
            await _hubContext.Clients.User(result.Project.ClientId.ToString())
                           .SendAsync("ClientNotification", "Proposal linked with your project has been deleted.");
        return result != null ? Success(result) : NotFound("Proposal not found or deletion failed.");
    }
}