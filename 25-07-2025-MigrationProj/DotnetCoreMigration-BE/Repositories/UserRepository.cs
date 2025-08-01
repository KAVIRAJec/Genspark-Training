using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class UserRepository : IRepository<User, int>
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Include(u => u.Orders)
            .Include(u => u.Products)
            .Include(u => u.News)
            .OrderByDescending(u => u.CreatedDate)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<User>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Users
            .Where(u => u.IsActive)
            .Include(u => u.Orders)
            .Include(u => u.Products)
            .Include(u => u.News)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => 
                u.UserName.ToLower().Contains(request.SearchTerm.ToLower()) ||
                u.Email.ToLower().Contains(request.SearchTerm.ToLower()) ||
                (u.FirstName != null && u.FirstName.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (u.LastName != null && u.LastName.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (u.Phone != null && u.Phone.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "username":
                    query = request.SortDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                    break;
                case "email":
                    query = request.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                    break;
                case "firstname":
                    query = request.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                    break;
                case "createddate":
                    query = request.SortDescending ? query.OrderByDescending(u => u.CreatedDate) : query.OrderBy(u => u.CreatedDate);
                    break;
                default:
                    query = query.OrderByDescending(u => u.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedDate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<User>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<User?> GetById(int id)
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .Include(u => u.Orders)
            .Include(u => u.Products)
            .Include(u => u.News)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }

    public async Task<User> Create(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> Update(User user)
    {
        var entry = _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        // Soft delete - set IsActive to false
        user.IsActive = false;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
