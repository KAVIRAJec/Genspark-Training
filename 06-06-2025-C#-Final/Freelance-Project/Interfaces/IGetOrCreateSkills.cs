using Freelance_Project.Models;
using Freelance_Project.Models.DTO;

namespace Freelance_Project.Interfaces;

public interface IGetOrCreateSkills
{
    public Task<List<Skill>> GetOrCreateSkills(IEnumerable<SkillDTO> skillDTOs);
}