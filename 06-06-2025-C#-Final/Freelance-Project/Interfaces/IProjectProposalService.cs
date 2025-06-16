using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IProjectProposalService
{
    public Task<PagedResponse<ProposalResponseDTO>> GetProposalsByProjectId(Guid projectId, PaginationParams paginationParams);
    public Task<ProjectResponseDTO> AcceptProposal(Guid proposalId, Guid projectId);
    // Change status to cancelled & set IsActive to false(Track delete after accept proposal)
    public Task<ProjectResponseDTO> CancelProject(Guid projectId);
    public Task<ProposalResponseDTO> RejectProposal(Guid proposalId, Guid projectId);
    public Task<ProjectResponseDTO> CompleteProject(Guid projectId);
} 