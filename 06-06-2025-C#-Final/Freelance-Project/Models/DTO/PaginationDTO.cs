namespace Freelance_Project.Models.DTO;

public class PaginationMetadata
{
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public PaginationMetadata Pagination { get; set; }
}
