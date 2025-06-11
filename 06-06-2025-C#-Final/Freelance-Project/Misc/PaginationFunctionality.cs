using Freelance_Project.Models;
using Freelance_Project.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Freelance_Project.Misc;

public static class IQueryableExtensions
{
    public static async Task<PagedResponse<T>> ToPagedResponse<T>(this IQueryable<T> query, PaginationParams paginationParams)
    {
        var totalRecords = await query.CountAsync();
        var items = await query
            .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync();

        var totalPages = (int)Math.Ceiling(totalRecords / (double)paginationParams.PageSize);

        if (paginationParams.Page > totalPages && totalPages > 0)
        {
            throw new AppException($"Page {paginationParams.Page} does not exist. Max page is {totalPages}.", 400);
        }

        return new PagedResponse<T>
        {
            Data = items,
            Pagination = new PaginationMetadata
            {
                TotalRecords = totalRecords,
                Page = paginationParams.Page,
                PageSize = paginationParams.PageSize,
                TotalPages = totalPages
            }
        };
    }
}
