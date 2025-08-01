using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class ModelRepository : IRepository<Model, int>
{
    private readonly ApplicationDbContext _context;

    public ModelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Model>> GetAll()
    {
        return await _context.Models
            .Where(m => m.IsActive)
            .Include(m => m.Products)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<Model>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Models
            .Where(m => m.IsActive)
            .Include(m => m.Products)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(m => m.Name.ToLower().Contains(request.SearchTerm.ToLower()));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "name":
                    query = request.SortDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name);
                    break;
                case "createddate":
                    query = request.SortDescending ? query.OrderByDescending(m => m.CreatedDate) : query.OrderBy(m => m.CreatedDate);
                    break;
                default:
                    query = query.OrderBy(m => m.Name);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(m => m.Name);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<Model>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Model?> GetById(int id)
    {
        return await _context.Models
            .Where(m => m.IsActive)
            .Include(m => m.Products)
            .FirstOrDefaultAsync(m => m.ModelId == id);
    }

    public async Task<Model> Create(Model model)
    {
        _context.Models.Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<Model> Update(Model model)
    {
        _context.Models.Update(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<bool> Delete(int id)
    {
        var model = await _context.Models.FindAsync(id);
        if (model == null) return false;

        // Soft delete - set IsActive to false
        model.IsActive = false;
        _context.Models.Update(model);
        await _context.SaveChangesAsync();
        return true;
    }
}
