using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class ClientRepository : Repository<Guid, Client>
{
    public ClientRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<Client> Delete(Guid key)
    {
        var client = await Get(key);
        if (client != null)
        {
            client.IsActive = false; // Soft delete
            client.DeletedAt = DateTime.UtcNow;
            client = await Update(client.Id, client);
            if (client == null) throw new AppException($"Failed to delete client.", 500);
            return client;
        }
        throw new AppException($"Client with ID {key} not found.", 404);
    }

    public override async Task<Client> Get(Guid key)
    {
        var client = await _appContext.Clients
                                        .Include(c => c.Projects)
                                        .SingleOrDefaultAsync(c => c.Id == key);
        return client ?? throw new AppException($"Client with ID {key} not found.", 404);
    }

    public override async Task<IEnumerable<Client>> GetAll()
    {
        return await _appContext.Clients
                                .Include(c => c.Projects)
                                .ToListAsync();
    }
}