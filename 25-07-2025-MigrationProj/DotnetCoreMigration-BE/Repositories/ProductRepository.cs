using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Repositories;

public class ProductRepository : IRepository<Product, int>
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAll()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Color)
            .Include(p => p.Model)
            .Include(p => p.User)
            .OrderByDescending(p => p.ProductId)
            .ToListAsync();
    }

    public async Task<Product?> GetById(int id)
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Color)
            .Include(p => p.Model)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }

    public async Task<Product> Create(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> Update(Product product)
    {
        var entry = _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        // Soft delete - set IsActive to false
        product.IsActive = false;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaginatedResponse<Product>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .Include(p => p.Color)
            .Include(p => p.Model)
            .Include(p => p.User)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => 
                p.ProductName.ToLower().Contains(request.SearchTerm.ToLower()) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (p.Color != null && p.Color.Name.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (p.Model != null && p.Model.Name.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "name":
                case "productname":
                    query = request.SortDescending ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName);
                    break;
                case "price":
                    query = request.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                    break;
                case "category":
                    query = request.SortDescending ? query.OrderByDescending(p => p.Category != null ? p.Category.Name : "") : query.OrderBy(p => p.Category != null ? p.Category.Name : "");
                    break;
                default:
                    query = query.OrderByDescending(p => p.ProductId);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(p => p.ProductId);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<Product>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
