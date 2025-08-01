using Microsoft.EntityFrameworkCore;
using DotnetCoreMigration.Data;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Repositories;

public class RefreshTokenRepository : IRepository<RefreshToken, int>
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RefreshToken>> GetAll()
    {
        return await _context.RefreshTokens
            .Where(rt => rt.IsActive)
            .Include(rt => rt.User)
            .OrderByDescending(rt => rt.CreatedDate)
            .ToListAsync();
    }

    public async Task<PaginatedResponse<RefreshToken>> GetAllPaginated(PaginationRequest request)
    {
        var query = _context.RefreshTokens
            .Where(rt => rt.IsActive)
            .Include(rt => rt.User)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(rt => 
                rt.Token.ToLower().Contains(request.SearchTerm.ToLower()) ||
                (rt.User != null && rt.User.UserName.ToLower().Contains(request.SearchTerm.ToLower())) ||
                (rt.User != null && rt.User.Email.ToLower().Contains(request.SearchTerm.ToLower())));
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "createddate":
                    query = request.SortDescending ? query.OrderByDescending(rt => rt.CreatedDate) : query.OrderBy(rt => rt.CreatedDate);
                    break;
                case "expiresat":
                case "expirationdate":
                    query = request.SortDescending ? query.OrderByDescending(rt => rt.ExpirationDate) : query.OrderBy(rt => rt.ExpirationDate);
                    break;
                case "user":
                    query = request.SortDescending ? query.OrderByDescending(rt => rt.User != null ? rt.User.UserName : "") : query.OrderBy(rt => rt.User != null ? rt.User.UserName : "");
                    break;
                default:
                    query = query.OrderByDescending(rt => rt.CreatedDate);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(rt => rt.CreatedDate);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PaginatedResponse<RefreshToken>(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<RefreshToken?> GetById(int id)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.IsActive)
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.RefreshTokenId == id);
    }

    public async Task<RefreshToken> Create(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken> Update(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<bool> Delete(int id)
    {
        var refreshToken = await _context.RefreshTokens.FindAsync(id);
        if (refreshToken == null) return false;

        // Soft delete - set IsActive to false
        refreshToken.IsActive = false;
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
        return true;
    }

    // Additional methods for refresh token management
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.IsActive && rt.Token == token)
            .Include(rt => rt.User)
            .FirstOrDefaultAsync();
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(int userId)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.IsActive && rt.UserId == userId)
            .Include(rt => rt.User)
            .OrderByDescending(rt => rt.CreatedDate)
            .ToListAsync();
    }

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await GetByTokenAsync(token);
        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            await Update(refreshToken);
        }
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var userTokens = await GetByUserIdAsync(userId);
        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
        }
        await _context.SaveChangesAsync();
    }
}
