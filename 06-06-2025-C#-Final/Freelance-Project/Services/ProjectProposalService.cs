using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Services;

public class ProjectProposalService : IProjectProposalService
{
    private readonly IRepository<Guid, Project> _projectRepository;
    private readonly IRepository<Guid, Proposal> _proposalRepository;
    private readonly IRepository<Guid, Freelancer> _freelancerRepository;
    private readonly FreelanceDBContext _dbContext;
    public ProjectProposalService(IRepository<Guid, Project> projectRepository,
                                  IRepository<Guid, Proposal> proposalRepository,
                                  IRepository<Guid, Freelancer> freelancerRepository,
                                  FreelanceDBContext freelanceDBContext)
    {
        _projectRepository = projectRepository;
        _proposalRepository = proposalRepository;
        _freelancerRepository = freelancerRepository;
        _dbContext = freelanceDBContext;
    }

    public async Task<PagedResponse<ProposalResponseDTO>> GetProposalsByProjectId(Guid projectId, PaginationParams paginationParams)
    {
        if (projectId == Guid.Empty) throw new AppException("Project ID cannot be empty.", 400);
        var project = await _projectRepository.Get(projectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);

        if (project.FreelancerId != null && project.FreelancerId != Guid.Empty)
        {
            var freelancer = await _freelancerRepository.Get((Guid)project.FreelancerId);
            if (freelancer == null || freelancer.IsActive == false) throw new AppException("Freelancer not found/ inactive.", 404);
        }

        var query = _dbContext.Proposals
            .Where(p => p.ProjectId == projectId)
            .Select(p => ProposalMapper.ToResponseDTO(p));

        return await query.ToPagedResponse(paginationParams);
    }

    public virtual async Task<ProjectResponseDTO> AcceptProposal(Guid proposalId, Guid projectId)
    {
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            if (proposalId == Guid.Empty || projectId == Guid.Empty) throw new AppException("Proposal ID or Project ID cannot be empty.", 400);

            var proposal = await _proposalRepository.Get(proposalId);
            if (proposal == null || proposal.IsActive == false) throw new AppException("Proposal not found/ inactive.", 404);
            if (proposal.IsAccepted == true) throw new AppException("Proposal is already accepted.", 400);
            if (proposal.IsAccepted == false) throw new AppException("Proposal is rejected, cannot accept.", 400);

            var project = await _projectRepository.Get(projectId);
            if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
            if (project.Status != "Pending") throw new AppException("Project is not in pending state.", 400);

            // Reject all other proposals for the project
            var allProposals = await _proposalRepository.GetAll();
            foreach (var p in allProposals.Where(p => p.ProjectId == projectId && p.Id != proposalId))
            {
                p.IsAccepted = false;
                p.UpdatedAt = DateTime.UtcNow;
                await _proposalRepository.Update(p.Id, p);
            }
            proposal.IsAccepted = true;
            proposal.UpdatedAt = DateTime.UtcNow;
            await _proposalRepository.Update(proposal.Id, proposal);

            project.Status = "In Progress";
            project.FreelancerId = proposal.FreelancerId;
            project.UpdatedAt = DateTime.UtcNow;
            var updatedProject = await _projectRepository.Update(project.Id, project);
            if (updatedProject == null) throw new AppException("Project update failed.", 500);

            await transaction.CommitAsync();
            return ProjectMapper.ToResponseDTO(updatedProject);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AppException($"Error occurred in AcceptProposal: {ex.Message}", 500);
        }
    }

    public virtual async Task<ProjectResponseDTO> CancelProject(Guid projectId)
    {
        using var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            if (projectId == Guid.Empty) throw new AppException("Project ID cannot be empty.", 400);

            var project = await _projectRepository.Get(projectId);
            if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
            if (project.Status == "Cancelled") throw new AppException("Project already cancelled.", 400);
            if (project.Status == "Completed") throw new AppException("Project already completed.", 400);
            if (project.Status == "Pending") throw new AppException("Project is not Approved, only Approved projects can be cancelled.", 400);
            // only allows in Progress
            project.Status = "Cancelled";
            project.UpdatedAt = DateTime.UtcNow;
            var updatedProject = await _projectRepository.Update(project.Id, project);
            if (updatedProject == null) throw new AppException("Project update failed.", 500);

            await transaction.CommitAsync();
            return ProjectMapper.ToResponseDTO(updatedProject);
        } catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new AppException($"Error occurred in CancelProject: {ex.Message}", 500);
        }
    }
    
    public async Task<ProposalResponseDTO> RejectProposal(Guid proposalId, Guid projectId)
    {
        if (proposalId == Guid.Empty || projectId == Guid.Empty) throw new AppException("Proposal ID or Project ID cannot be empty.", 400);

        var proposal = await _proposalRepository.Get(proposalId);
        if (proposal == null || proposal.IsActive == false) throw new AppException("Proposal not found/ inactive.", 404);
        if (proposal.Id == proposalId && proposal.ProjectId != projectId) 
            throw new AppException("Proposal does not belong to the specified project.", 400);
        if (proposal.IsAccepted == true) throw new AppException("Cannot reject an accepted proposal.", 400);

        var project = await _projectRepository.Get(projectId);
        if (project == null || project.IsActive == false) throw new AppException("Project not found/ inactive.", 404);
        if (project.Status != "Pending") throw new AppException("Project is not in pending state.", 400);

        proposal.IsAccepted = false;
        var updatedProposal = await _proposalRepository.Update(proposal.Id, proposal);
        if (updatedProposal == null) throw new AppException("Proposal update failed.", 500);

        return ProposalMapper.ToResponseDTO(updatedProposal);
    }
}