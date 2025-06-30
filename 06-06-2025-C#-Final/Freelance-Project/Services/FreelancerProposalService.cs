using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class FreelancerProposalService : IFreelancerProposalService
{
    private readonly IRepository<Guid, Proposal> _proposalRepository;
    private readonly IRepository<Guid, Freelancer> _freelancerRepository;
    private readonly IRepository<Guid, Client> _clientRepository;
    private readonly IRepository<Guid, Project> _projectRepository;
    private readonly FreelanceDBContext _context;

    public FreelancerProposalService(IRepository<Guid, Proposal> proposalRepository,
                                      IRepository<Guid, Freelancer> freelancerRepository,
                                      IRepository<Guid, Client> clientRepository,
                                      IRepository<Guid, Project> projectRepository,
                                      FreelanceDBContext freelancerDbContext)
    {
        _proposalRepository = proposalRepository;
        _freelancerRepository = freelancerRepository;
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
        _context = freelancerDbContext;
    }

    public async Task<ProposalResponseDTO> CreateProposal(CreateProposalDTO proposalRequestDto)
    {
        if (proposalRequestDto == null) throw new AppException("Proposal request cannot be null.");
        if (proposalRequestDto.FreelancerId == Guid.Empty) throw new AppException("Freelancer ID cannot be empty.");

        var freelancer = await _freelancerRepository.Get(proposalRequestDto.FreelancerId);
        if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);

        var project = await _projectRepository.Get(proposalRequestDto.ProjectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);

        if (project.FreelancerId != null && project.FreelancerId == proposalRequestDto.FreelancerId)
            throw new AppException("Freelancer cannot submit a proposal for their own project.", 400);

        if (project.FreelancerId != null) throw new AppException("Project already has an assigned freelancer.", 400);

        if (project.Proposals != null && project.Proposals.Any(p => p.FreelancerId == proposalRequestDto.FreelancerId && p.IsActive == true))
            throw new AppException("Freelancer has already submitted a proposal for this project.", 400);

        var proposal = ProposalMapper.CreateProposalFromCreateDTO(proposalRequestDto);

        var createdProposal = await _proposalRepository.Add(proposal);
        if (createdProposal == null) throw new AppException("Unable to create proposal.", 500);

        return ProposalMapper.ToResponseDTO(createdProposal);
    }

    public async Task<ProposalResponseDTO> DeleteProposal(Guid proposalId)
    {
        if (proposalId == Guid.Empty) throw new AppException("Proposal ID cannot be empty.", 400);

        var proposal = await _proposalRepository.Get(proposalId);
        if (proposal == null || proposal.IsActive == false) throw new AppException("Proposal not found/ inactive.", 404);
        var project = await _projectRepository.Get(proposal.ProjectId);

        var deletedProposal = await _proposalRepository.Delete(proposal.Id);
        if (deletedProposal == null) throw new AppException("Unable to delete proposal.", 500);
        return ProposalMapper.ToResponseDTO(deletedProposal);
    }

    public async Task<PagedResponse<ProposalResponseDTO>> GetAllProposalsPaged(PaginationParams paginationParams)
    {
        var query = _context.Proposals
            .Where(p => p.IsActive == true)
            .Include(p => p.Freelancer)
            .Include(p => p.Project)
            .Select(p => p);

        if (!string.IsNullOrEmpty(paginationParams.Search))
            query = query.Where(p => p.Description.Contains(paginationParams.Search) ||
                                     p.Freelancer.Username.Contains(paginationParams.Search));

        if (!string.IsNullOrEmpty(paginationParams.SortBy))
        {
            switch (paginationParams.SortBy.ToLower())
            {
                case "createdat":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                case "proposedamount":
                    query = query.OrderByDescending(p => p.ProposedAmount);
                    break;
                case "proposedduration":
                    query = query.OrderByDescending(p => p.ProposedDuration);
                    break;
                case "isaccepted":
                    query = query.OrderByDescending(p => p.IsAccepted);
                    break;
                case "isrejected":
                    query = query.OrderByDescending(p => p.IsRejected);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        var result = query.Select(p => ProposalMapper.ToResponseDTO(p));
        return await result.ToPagedResponse(paginationParams);
    }

    public async Task<ProposalResponseDTO> GetProposalById(Guid proposalId)
    {
        if (proposalId == Guid.Empty) throw new AppException("Proposal ID is required.", 400);
        var proposal = await _proposalRepository.Get(proposalId);
        if (proposal == null || proposal.IsActive == false) throw new AppException("Proposal not found/ inactive.", 404);

        return ProposalMapper.ToResponseDTO(proposal);
    }

    public async Task<PagedResponse<ProposalResponseDTO>> GetProposalsByFreelancerId(Guid freelancerId, PaginationParams paginationParams)
    {
        if (freelancerId == Guid.Empty) throw new AppException("Freelancer ID is required.", 400);
        var freelancer = await _freelancerRepository.Get(freelancerId);
        if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);

        var query = _context.Proposals
            .Where(p => p.FreelancerId == freelancerId && p.IsActive == true)
            .Include(p => p.Freelancer)
            .Include(p => p.Project)
            .Select(p => p);

        if (!string.IsNullOrEmpty(paginationParams.Search))
            query = query.Where(p => p.Description.Contains(paginationParams.Search) ||
                                     p.Freelancer.Username.Contains(paginationParams.Search));

        if (!string.IsNullOrEmpty(paginationParams.SortBy))
        {
            switch (paginationParams.SortBy.ToLower())
            {
                case "createdat":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                case "proposedamount":
                    query = query.OrderByDescending(p => p.ProposedAmount);
                    break;
                case "proposedduration":
                    query = query.OrderByDescending(p => p.ProposedDuration);
                    break;
                case "isaccepted":
                    query = query.OrderByDescending(p => p.IsAccepted);
                    break;
                case "isrejected":
                    query = query.OrderByDescending(p => p.IsRejected);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        var result = query.Select(p => ProposalMapper.ToResponseDTO(p));
        return await result.ToPagedResponse(paginationParams);
    }

    public async Task<PagedResponse<ProposalResponseDTO>> GetProposalsByClientId(Guid clientId, PaginationParams paginationParams)
    {
        if (clientId == Guid.Empty) throw new AppException("Client ID is required.", 400);
        var client = await _clientRepository.Get(clientId);
        if (client == null || client.IsActive == false) throw new AppException("Client not found/ inactive.", 404);

        var query = _context.Proposals
            .Where(p => p.Project.ClientId == clientId && p.IsActive == true)
            .Include(p => p.Freelancer)
            .Include(p => p.Project)
            .Select(p => p);

        if (!string.IsNullOrEmpty(paginationParams.Search))
            query = query.Where(p => p.Description.Contains(paginationParams.Search) ||
                                     p.Freelancer.Username.Contains(paginationParams.Search) ||
                                     p.Project.Title.Contains(paginationParams.Search) ||
                                     p.Project.Description.Contains(paginationParams.Search));

        if (!string.IsNullOrEmpty(paginationParams.SortBy))
        {
            switch (paginationParams.SortBy.ToLower())
            {
                case "createdat":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                case "projecttitle":
                    query = query.OrderByDescending(p => p.Project.Title);
                    break;
                case "proposedamount":
                    query = query.OrderByDescending(p => p.ProposedAmount);
                    break;
                case "proposedduration":
                    query = query.OrderByDescending(p => p.ProposedDuration);
                    break;
                case "isaccepted":
                    query = query.OrderByDescending(p => p.IsAccepted);
                    break;
                case "isrejected":
                    query = query.OrderByDescending(p => p.IsRejected);
                    break;
                default:
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.CreatedAt);
        }

        var result = query.Select(p => ProposalMapper.ToResponseDTO(p));
        return await result.ToPagedResponse(paginationParams);
    }

    public async Task<ProposalResponseDTO> UpdateProposal(Guid proposalId, UpdateProposalDTO updateDto)
    {
        if (proposalId == Guid.Empty) throw new AppException("Proposal ID is required.", 400);
        if (updateDto == null) throw new AppException("Update data is required.", 400);

        var proposal = await _proposalRepository.Get(proposalId);
        if (proposal == null || proposal.IsActive == false) throw new AppException("Proposal not found/ inactive.", 404);
        var project = await _projectRepository.Get(proposal.ProjectId);

        proposal.Description = updateDto.Description ?? proposal.Description;
        proposal.ProposedAmount = updateDto.ProposedAmount ?? proposal.ProposedAmount;
        proposal.ProposedDuration = updateDto.ProposedDuration ?? proposal.ProposedDuration;
        proposal.UpdatedAt = DateTime.UtcNow;


        var updatedProposal = await _proposalRepository.Update(proposal.Id, proposal);
        if (updatedProposal == null) throw new AppException("Proposal update failed.", 500);
        return ProposalMapper.ToResponseDTO(updatedProposal);
    }
}