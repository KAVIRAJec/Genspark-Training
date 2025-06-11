using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class SkillRepository : Repository<Guid, Skill>
{
    public SkillRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<Skill> Delete(Guid key)
    {
        var skill = await Get(key);
        if (skill != null)
        {
            skill.IsActive = false; // Soft delete
            skill.DeletedAt = DateTime.Now;
            skill = await Update(skill.Id, skill);
            if (skill == null) throw new AppException("Failed to delete skill.", 500);
            return skill;
        }
        throw new AppException($"Skill with ID {key} not found.", 404);
    }

    public override async Task<Skill> Get(Guid key)
    {
        var skill = await _appContext.Skills
                                        .Include(s => s.Projects)
                                        .Include(s => s.Freelancers)
                                        .SingleOrDefaultAsync(s => s.Id == key);
        return skill ?? throw new AppException($"Skill with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<Skill>> GetAll()
    {
        return await _appContext.Skills
                                .Include(s => s.Projects)
                                .Include(s => s.Freelancers)
                                .ToListAsync();
    }
}