using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Freelance_Project.Misc;

public static class ProposalMapper
{
    public static ProposalResponseDTO ToResponseDTO(Proposal proposal)
    {
        return new ProposalResponseDTO
        {
            Id = proposal.Id,
            Description = proposal.Description,
            ProposedAmount = proposal.ProposedAmount,
            ProposedDuration = proposal.ProposedDuration ?? TimeSpan.Zero,
            IsActive = proposal.IsActive,
            IsAccepted = proposal.IsAccepted ?? false,
            IsRejected = proposal.IsRejected ?? false,
            CreatedAt = proposal.CreatedAt,
            UpdatedAt = proposal.UpdatedAt,
            DeletedAt = proposal.DeletedAt,
            Freelancer = proposal.Freelancer != null ?
                new FreelancerSummaryDTO
                {
                    Id = proposal.Freelancer.Id,
                    Username = proposal.Freelancer.Username,
                    Email = proposal.Freelancer.Email,
                    IsActive = proposal.Freelancer.IsActive
                }
                : new FreelancerSummaryDTO(),
            Project = proposal.Project != null ?
            new ProjectSummaryDTO
            {
                Id = proposal.Project.Id,
                Title = proposal.Project.Title,
                Status = proposal.Project.Status,
                IsActive = proposal.Project.IsActive,
                FreelancerId = proposal.Project.FreelancerId,
                ClientId = proposal.Project.ClientId
            }
                : new ProjectSummaryDTO()
        };
    }
    public static Proposal CreateProposalFromCreateDTO(CreateProposalDTO proposalDto)
    {
        return new Proposal
        {
            Description = proposalDto.Description,
            ProposedAmount = proposalDto.ProposedAmount,
            ProposedDuration = proposalDto.ProposedDuration ?? null,
            IsActive = true,
            FreelancerId = proposalDto.FreelancerId,
            ProjectId = proposalDto.ProjectId,
            CreatedAt = DateTime.UtcNow
        };
    }
}