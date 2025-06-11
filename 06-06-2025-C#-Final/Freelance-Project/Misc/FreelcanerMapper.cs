using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Misc;

public static class FreelancerMapper
{
    public static FreelancerResponseDTO ToResponseDTO(Freelancer freelancer)
    {
        return new FreelancerResponseDTO
        {
            Id = freelancer.Id,
            ProfileUrl = freelancer.ProfileUrl,
            Username = freelancer.Username,
            Email = freelancer.Email,
            ExperienceYears = freelancer.ExperienceYears,
            HourlyRate = freelancer.HourlyRate,
            Location = freelancer.Location,
            IsActive = freelancer.IsActive,
            CreatedAt = freelancer.CreatedAt,
            UpdatedAt = freelancer.UpdatedAt,
            DeletedAt = freelancer.DeletedAt,
            Skills = freelancer.Skills?.Select(s => new SkillDTO { SkillId = s.Id, Name = s.Name }).ToList()
        };
    }
    public static Freelancer CreateFreelancerFromCreateDTO(CreateFreelancerDTO createDto, List<Skill> skills)
    {
        if (createDto == null) throw new AppException("CreateFreelancerDTO cannot be null.", 400);  
        return new Freelancer
        {
            ProfileUrl = createDto.ProfileUrl ?? null,
            Username = createDto.Username,
            Email = createDto.Email,
            ExperienceYears = createDto.ExperienceYears,
            HourlyRate = createDto.HourlyRate,
            Location = createDto.Location ?? null,
            IsActive = true,
            Skills = skills
        };
    }
}