using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IFreelancerProposalService
{
    public Task<ProposalResponseDTO> CreateProposal(CreateProposalDTO proposalRequestDto);
    public Task<PagedResponse<ProposalResponseDTO>> GetAllProposalsPaged(PaginationParams paginationParams);
    public Task<ProposalResponseDTO> GetProposalById(Guid proposalId);
    public Task<PagedResponse<ProposalResponseDTO>> GetProposalsByFreelancerId(Guid freelancerId, PaginationParams paginationParams);
    public Task<PagedResponse<ProposalResponseDTO>> GetProposalsByClientId(Guid clientId, PaginationParams paginationParams);
    public Task<ProposalResponseDTO> UpdateProposal(Guid proposalId, UpdateProposalDTO updateDto);
    public Task<ProposalResponseDTO> DeleteProposal(Guid proposalId);
}