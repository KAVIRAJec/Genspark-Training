using Freelance_Project.Interfaces;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ClientController : BaseApiController
{
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateClient([FromBody] CreateClientDTO createClientDTO)
    {
        if (createClientDTO == null) return BadRequest("CreateClientDTO cannot be null");
        var result = await _clientService.CreateClient(createClientDTO);
        return result != null ? Success(result) : BadRequest("Client creation failed");
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClientById(Guid clientId)
    {
        var result = await _clientService.GetClientById(clientId);
        return result != null ? Success(result) : NotFound("Client not found");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllClients([FromQuery] PaginationParams paginationParams)
    {
        var result = await _clientService.GetAllClientsPaged(paginationParams);
        return result != null ? Success(result) : NotFound("No clients found");
    }

    [HttpPut("{clientId}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> UpdateClient([FromRoute] Guid clientId, [FromBody] UpdateClientDTO updateClientDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if(Id != clientId.ToString()) return BadRequest("You are not authorized to update other client");

        var result = await _clientService.UpdateClient(clientId, updateClientDTO);
        return result != null ? Success(result) : NotFound("Unable to update client");
    }

    [HttpDelete("{clientId}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> DeleteClient([FromRoute] Guid clientId)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if(Id != clientId.ToString()) return BadRequest("You are not authorized to delete other client");

        var result = await _clientService.DeleteClient(clientId);
        return result != null ? Success(result) : NotFound("Unable to delete client");
    }
}