using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class ProposalRepository : Repository<Guid, Proposal>
{
    public ProposalRepository(FreelanceDBContext context) : base(context)
    {
    }

    public override async Task<Proposal> Delete(Guid key)
    {
        var proposal = await Get(key);
        if (proposal != null)
        {
            proposal.IsActive = false; // Soft delete
            proposal.DeletedAt = DateTime.UtcNow;
            proposal = await Update(proposal.Id, proposal);
            if(proposal == null) throw new AppException("Proposal could not be deleted.", 500);
            return proposal;
        }
        throw new AppException("Proposal not found.", 404);

    }

    public async override Task<Proposal> Get(Guid key)
    {
        var proposal = await _appContext.Proposals
                                        .Include(p => p.Freelancer)
                                        .Include(p => p.Project)
                                        .FirstOrDefaultAsync(p => p.Id == key);
        return proposal ?? throw new AppException("Proposal not found.", 404);
    }

    public override async Task<IEnumerable<Proposal>> GetAll()
    {
        return await _appContext.Proposals
                                 .Include(p => p.Freelancer)
                                 .Include(p => p.Project)
                                 .ToListAsync();
    }
}