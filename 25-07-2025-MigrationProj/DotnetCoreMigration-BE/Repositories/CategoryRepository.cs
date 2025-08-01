using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class CategoryRepository : IRepository<Category, int>
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<Category>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Categories
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

        return new PaginatedResponse<Category>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Category?> GetById(int id)
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
    }

    public async Task<Category> Create(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> Update(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        // Soft delete - set IsActive to false
        category.IsActive = false;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return true;
    }
}
