using Freelance_Project.Interfaces;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class FreelancerController : BaseApiController
{
    private readonly IFreelancerService _freelancerService;

    public FreelancerController(IFreelancerService freelancerService)
    {
        _freelancerService = freelancerService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateFreelancer([FromBody] CreateFreelancerDTO createFreelancerDTO)
    {
        if (createFreelancerDTO == null) return BadRequest("CreateFreelancerDTO cannot be null");
        var result = await _freelancerService.CreateFreelancer(createFreelancerDTO);
        return result != null ? Success(result) : BadRequest("Freelancer creation failed");
    }

    [HttpGet("{freelancerId}")]
    public async Task<IActionResult> GetFreelancerById(Guid freelancerId)
    {
        var result = await _freelancerService.GetFreelancerById(freelancerId);
        return result != null ? Success(result) : NotFound("Freelancer not found");
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAllFreelancers([FromQuery] PaginationParams paginationParams)
    {
        var result = await _freelancerService.GetAllFreelancersPaged(paginationParams);
        return result != null ? Success(result) : NotFound("No freelancers found");
    }

    [HttpPut("{freelancerId}")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> UpdateFreelancer([FromRoute] Guid freelancerId, [FromBody] UpdateFreelancerDTO updateFreelancerDTO)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        // Console.WriteLine($"User ID from claims: {Id}");
        if (Id != freelancerId.ToString()) return BadRequest("You are not authorized to update other freelancer");
        
        var result = await _freelancerService.UpdateFreelancer(freelancerId, updateFreelancerDTO);
        return result != null ? Success(result) : NotFound("Unable to update freelancer");
    }

    [HttpDelete("{freelancerId}")]
    [Authorize(Roles = "Freelancer")]
    public async Task<IActionResult> DeleteFreelancer([FromRoute] Guid freelancerId)
    {
        var Id = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (Id != freelancerId.ToString()) return BadRequest("You are not authorized to delete other freelancer");
        
        var result = await _freelancerService.DeleteFreelancer(freelancerId);
        return result != null ? Success(result) : NotFound("Unable to delete freelancer");
    }
}