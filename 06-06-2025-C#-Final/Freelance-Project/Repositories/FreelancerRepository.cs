using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class FreelancerRepository : Repository<Guid, Freelancer>
{
    public FreelancerRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<Freelancer> Delete(Guid key)
    {
        var freelancer = await Get(key);
        if (freelancer != null)
        {
            freelancer.IsActive = false; // Soft delete
            freelancer.DeletedAt = DateTime.UtcNow;
            freelancer = await Update(freelancer.Id, freelancer);
            if (freelancer == null) throw new AppException($"Failed to delete freelancer.", 500);
            return freelancer;
        }
        throw new AppException($"Freelancer with ID {key} not found.", 404);
    }

    public override async Task<Freelancer> Get(Guid key)
    {
        var freelancer = await _appContext.Freelancers
                                            .Include(f => f.Skills)
                                            .SingleOrDefaultAsync(f => f.Id == key);
        return freelancer ?? throw new AppException($"Freelancer with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<Freelancer>> GetAll()
    {
        return await _appContext.Freelancers
                                 .Include(f => f.Skills)
                                 .ToListAsync();
    }
}