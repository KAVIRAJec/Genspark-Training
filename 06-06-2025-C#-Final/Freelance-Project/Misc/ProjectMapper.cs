using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Misc;

public static class ProjectMapper
{
    public static ProjectResponseDTO ToResponseDTO(Project project)
    {
        return new ProjectResponseDTO
        {
            Id = project.Id,
            Title = project.Title,
            Description = project.Description,
            Budget = project.Budget,
            Duration = project.Duration,
            Status = project.Status,
            IsActive = project.IsActive,
            CreatedAt = project.CreatedAt,
            UpdatedAt = project.UpdatedAt,
            DeletedAt = project.DeletedAt,
            ClientId = project.ClientId,
            FreelancerId = project.FreelancerId,
            Proposals = project.Proposals?.Select(p => new ProposalSummaryDTO
            {
                Id = p.Id,
                FreelancerId = p.FreelancerId,
                ProposedAmount = p.ProposedAmount,
                ProposedDuration = p.ProposedDuration,
                IsActive = p.IsActive
            }).Where(p => p.IsActive == true).ToList() ?? new List<ProposalSummaryDTO>(),
            RequiredSkills = project.RequiredSkills?.Select(skill => new SkillDTO
            {
                SkillId = skill.Id,
                Name = skill.Name
            }).ToList() ?? new List<SkillDTO>(),
        };
    }
    public static Project CreateProjectFromCreateDTO(CreateProjectDTO projectDTO, List<Skill> requiredSkills)
    {
        if (projectDTO == null) throw new AppException("Project DTO cannot be null.", 400);

        return new Project
        {
            Title = projectDTO.Title,
            Description = projectDTO.Description,
            Budget = projectDTO.Budget,
            Duration = projectDTO.Duration ?? TimeSpan.Zero,
            ClientId = projectDTO.ClientId,
            RequiredSkills = requiredSkills,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}