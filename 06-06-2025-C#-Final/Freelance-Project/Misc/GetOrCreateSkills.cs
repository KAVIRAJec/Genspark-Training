using Freelance_Project.Contexts;
using Freelance_Project.Interfaces;
using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Services;

public class GetOrCreateSkillService : IGetOrCreateSkills
{
    private readonly FreelanceDBContext _dbContext;
    public GetOrCreateSkillService(FreelanceDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Skill>> GetOrCreateSkills(IEnumerable<SkillDTO> skillDTOs)
{
    var skills = new List<Skill>();
    foreach (var skillDto in skillDTOs)
    {
        var existingSkill = await _dbContext.Skills
            .FirstOrDefaultAsync(s => s.Name.ToLower() == skillDto.Name.ToLower());

        if (existingSkill != null)
        {
            skills.Add(existingSkill);
        }
        else
        {
            var newSkill = new Skill { Name = skillDto.Name };
            _dbContext.Skills.Add(newSkill);
            await _dbContext.SaveChangesAsync();
            skills.Add(newSkill);
        }
    }
    return skills;
    }
}