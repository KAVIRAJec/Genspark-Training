using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class NewsRepository : IRepository<News, int>
{
    private readonly ApplicationDbContext _context;

    public NewsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<News>> GetAll()
    {
        return await _context.News
            .Where(n => n.IsActive)
            .Include(n => n.Author)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<News>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.News
            .Where(n => n.IsActive)
            .Include(n => n.Author)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(n => 
                n.Title.ToLower().Contains(request.SearchTerm.ToLower()) ||
                (n.Content != null && n.Content.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (n.Author != null && n.Author.UserName.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "title":
                    query = request.SortDescending ? query.OrderByDescending(n => n.Title) : query.OrderBy(n => n.Title);
                    break;
                case "createddate":
                    query = request.SortDescending ? query.OrderByDescending(n => n.CreatedDate) : query.OrderBy(n => n.CreatedDate);
                    break;
                case "author":
                    query = request.SortDescending ? query.OrderByDescending(n => n.Author != null ? n.Author.UserName : "") : query.OrderBy(n => n.Author != null ? n.Author.UserName : "");
                    break;
                default:
                    query = query.OrderByDescending(n => n.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(n => n.CreatedDate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<News>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<News?> GetById(int id)
    {
        return await _context.News
            .Where(n => n.IsActive)
            .Include(n => n.Author)
            .FirstOrDefaultAsync(n => n.NewsId == id);
    }

    public async Task<News> Create(News news)
    {
        _context.News.Add(news);
        await _context.SaveChangesAsync();
        return news;
    }

    public async Task<News> Update(News news)
    {
        _context.News.Update(news);
        await _context.SaveChangesAsync();
        return news;
    }

    public async Task<bool> Delete(int id)
    {
        var news = await _context.News.FindAsync(id);
        if (news == null) return false;

        // Soft delete - set IsActive to false
        news.IsActive = false;
        _context.News.Update(news);
        await _context.SaveChangesAsync();
        return true;
    }
}
