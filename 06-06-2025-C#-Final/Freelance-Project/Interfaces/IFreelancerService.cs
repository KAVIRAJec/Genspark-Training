using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IFreelancerService
{
    public Task<FreelancerResponseDTO> CreateFreelancer(CreateFreelancerDTO createDto);
    public Task<PagedResponse<FreelancerResponseDTO>> GetAllFreelancersPaged(PaginationParams paginationParams);
    public Task<FreelancerResponseDTO> GetFreelancerById(Guid freelancerId);
    public Task<FreelancerResponseDTO> UpdateFreelancer(Guid freelancerId, UpdateFreelancerDTO updateDto);
    public Task<FreelancerResponseDTO> DeleteFreelancer(Guid freelancerId);
}