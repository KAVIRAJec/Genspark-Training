using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class ProjectRepository : Repository<Guid, Project>
{
    public ProjectRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<Project> Delete(Guid key)
    {
        var project = await Get(key);
        if (project != null)
        {
            project.IsActive = false;
            project.DeletedAt = DateTime.UtcNow;
            project = await Update(project.Id, project);
            if(project == null) throw new AppException("Failed to update project status.", 500);
            return project;
        }
        throw new AppException($"Project with ID {key} not found.", 404);
    }

    public override async Task<Project> Get(Guid key)
    {
        var project = await _appContext.Projects
                                        .Include(p => p.Client)
                                        .Include(p => p.Freelancer)
                                        .Include(p => p.Proposals)
                                        .Include(p => p.RequiredSkills)
                                        .SingleOrDefaultAsync(p => p.Id == key);
        return project ?? throw new AppException($"Project with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<Project>> GetAll()
    {
        return await _appContext.Projects
                                        .Include(p => p.Client)
                                        .Include(p => p.Freelancer)
                                        .Include(p => p.Proposals)
                                        .Include(p => p.RequiredSkills)
                                        .ToListAsync();
    }
}