using Microsoft.AspNetCore.Mvc;

namespace Freelance_Project.Models;

public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    [FromQuery(Name = "page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "pageSize")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    [FromQuery(Name = "search")]
    public string? Search { get; set; } = string.Empty;

    [FromQuery(Name = "sortBy")]
    public string? SortBy { get; set; } = "createdAt";
}
