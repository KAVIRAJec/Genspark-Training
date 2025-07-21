using Microsoft.EntityFrameworkCore;
using VideoStreamingPlatform.Data;
using VideoStreamingPlatform.Interfaces;
using VideoStreamingPlatform.Models;
using System.Linq.Expressions;

namespace VideoStreamingPlatform.Repositories;

public class TrainingVideoRepository : ITrainingVideoRepository
{
    private readonly TrainingVideoContext _context;

    public TrainingVideoRepository(TrainingVideoContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrainingVideo>> GetAllAsync()
    {
        return await _context.TrainingVideos
            .Where(v => v.IsActive)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<TrainingVideo?> GetByIdAsync(int id)
    {
        return await _context.TrainingVideos
            .FirstOrDefaultAsync(v => v.Id == id && v.IsActive);
    }

    public async Task<TrainingVideo> CreateAsync(TrainingVideo video)
    {
        video.CreatedAt = DateTime.UtcNow;
        video.UploadDate = DateTime.UtcNow;
        video.IsActive = true;
        
        _context.TrainingVideos.Add(video);
        await _context.SaveChangesAsync();
        return video;
    }

    public async Task<TrainingVideo> UpdateAsync(TrainingVideo video)
    {
        video.UpdatedAt = DateTime.UtcNow;
        _context.TrainingVideos.Update(video);
        await _context.SaveChangesAsync();
        return video;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var video = await GetByIdAsync(id);
        if (video == null) return false;
        
        // Soft delete
        video.IsActive = false;
        video.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(IEnumerable<TrainingVideo> Videos, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? searchTerm = null, 
        string? sortBy = null, 
        bool sortDescending = true)
    {
        var query = _context.TrainingVideos.Where(v => v.IsActive);

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(v => 
                v.Title.Contains(searchTerm) || 
                (v.Description != null && v.Description.Contains(searchTerm)));
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, sortBy, sortDescending);

        // Apply pagination
        var videos = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (videos, totalCount);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TrainingVideos
            .AnyAsync(v => v.Id == id && v.IsActive);
    }

    private IQueryable<TrainingVideo> ApplySorting(IQueryable<TrainingVideo> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "title" => sortDescending ? query.OrderByDescending(v => v.Title) : query.OrderBy(v => v.Title),
            "uploaddate" => sortDescending ? query.OrderByDescending(v => v.UploadDate) : query.OrderBy(v => v.UploadDate),
            "filesize" => sortDescending ? query.OrderByDescending(v => v.FileSize) : query.OrderBy(v => v.FileSize),
            "createdat" => sortDescending ? query.OrderByDescending(v => v.CreatedAt) : query.OrderBy(v => v.CreatedAt),
            _ => query.OrderByDescending(v => v.CreatedAt)
        };
    }
}
