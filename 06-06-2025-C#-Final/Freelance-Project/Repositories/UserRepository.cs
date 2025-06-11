using Freelance_Project.Contexts;
using Freelance_Project.Misc;
using Freelance_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Repositories;

public class UserRepository : Repository<string, User>
{
    public UserRepository(FreelanceDBContext appContext) : base(appContext)
    {
    }

    public override async Task<User> Delete(string email)
    {
        var user = await Get(email);
        if (user != null)
        {
            user.IsActive = false; // Soft delete
            user.DeletedAt = DateTime.UtcNow;
            user = await Update(user.Email, user);
            if (user == null) throw new AppException("Failed to delete user.", 500);
            return user;
        }
        throw new AppException($"User with email {email} not found.", 404);
    }

    public override async Task<User> Get(string email)
    {
        var user = await _appContext.Users.FindAsync(email);
        return user ?? throw new AppException($"User with email {email} not found.", 404);
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        return await _appContext.Users.ToListAsync();
    }
}