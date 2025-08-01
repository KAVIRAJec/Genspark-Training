using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class ColorRepository : IRepository<Color, int>
{
    private readonly ApplicationDbContext _context;

    public ColorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Color>> GetAll()
    {
        return await _context.Colors
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<Color>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Colors
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => c.Name.ToLower().Contains(request.SearchTerm.ToLower()));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "name":
                    query = request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(c => c.Name);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<Color>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Color?> GetById(int id)
    {
        return await _context.Colors
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.ColorId == id);
    }

    public async Task<Color> Create(Color color)
    {
        _context.Colors.Add(color);
        await _context.SaveChangesAsync();
        return color;
    }

    public async Task<Color> Update(Color color)
    {
        _context.Colors.Update(color);
        await _context.SaveChangesAsync();
        return color;
    }

    public async Task<bool> Delete(int id)
    {
        var color = await _context.Colors.FindAsync(id);
        if (color == null) return false;

        // Soft delete - set IsActive to false
        color.IsActive = false;
        _context.Colors.Update(color);
        await _context.SaveChangesAsync();
        return true;
    }
}
