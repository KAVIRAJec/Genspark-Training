using FileApp.Repositories;
using FileApp.Contexts;
using FileApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FileApp.Repositories;

public class UserRepository : Repository<string, User>
{
    public UserRepository(FileAppContext fileAppContext) : base(fileAppContext)
    {
    }
    public override async Task<User> Get(string username)
    {
        var user = await _fileAppContext.Users.Include(u => u.UploadedFiles).FirstOrDefaultAsync(u => u.UserName == username);
        return user ?? throw new KeyNotFoundException($"User with username {username} not found.");
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        var users = await _fileAppContext.Users.Include(u => u.UploadedFiles).ToListAsync();
        return users ?? new List<User>();
    }
}