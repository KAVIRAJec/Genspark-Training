using System.ComponentModel.DataAnnotations;

namespace VideoStreamingPlatform.DTOs;

public class VideoUploadRequestDto
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Video file is required")]
    public IFormFile VideoFile { get; set; } = null!;
}

public class VideoResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UploadDate { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public long FileSize { get; set; }
    public string? ContentType { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class VideoListResponseDto
{
    public List<VideoResponseDto> Videos { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class VideoStreamResponseDto
{
    public Stream VideoStream { get; set; } = null!;
    public string ContentType { get; set; } = string.Empty;
    public long ContentLength { get; set; }
    public string FileName { get; set; } = string.Empty;
}

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class PaginationRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
