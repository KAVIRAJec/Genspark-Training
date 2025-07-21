using Microsoft.AspNetCore.Mvc;
using VideoStreamingPlatform.DTOs;
using VideoStreamingPlatform.Interfaces;

namespace VideoStreamingPlatform.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VideosController : ControllerBase
{
    private readonly ITrainingVideoService _videoService;
    private readonly ILogger<VideosController> _logger;

    public VideosController(ITrainingVideoService videoService, ILogger<VideosController> logger)
    {
        _videoService = videoService;
        _logger = logger;
    }

    /// Upload a new training video
    /// <param name="request">Video upload request containing title, description, and video file</param>
    /// <returns>Uploaded video information</returns>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponseDto<VideoResponseDto>>> UploadVideo([FromForm] VideoUploadRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? Array.Empty<string>()).ToList();
            return BadRequest(new ApiResponseDto<VideoResponseDto>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            });
        }

        var result = await _videoService.UploadVideoAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get all videos with pagination and filtering
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10)</param>
    /// <param name="searchTerm">Search term for title and description</param>
    /// <param name="sortBy">Sort field (title, uploaddate, filesize, createdat)</param>
    /// <param name="sortDescending">Sort order (default: true)</param>
    /// <returns>Paginated list of videos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseDto<VideoListResponseDto>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponseDto<VideoListResponseDto>>> GetVideos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] bool sortDescending = true)
    {
        var pagination = new PaginationRequestDto
        {
            Page = page,
            PageSize = Math.Min(pageSize, 50), // Limit max page size
            SearchTerm = searchTerm,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _videoService.GetVideosAsync(pagination);
        return Ok(result);
    }

    /// <summary>
    /// Get a specific video by ID
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <returns>Video information</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponseDto<VideoResponseDto>>> GetVideo(int id)
    {
        var result = await _videoService.GetVideoByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Stream a video file
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <returns>Video stream</returns>
    [HttpGet("{id:int}/stream")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> StreamVideo(int id)
    {
        var result = await _videoService.GetVideoStreamAsync(id);
        
        if (!result.Success || result.Data == null)
        {
            return NotFound(new { message = result.Message });
        }

        return File(
            result.Data.VideoStream,
            result.Data.ContentType,
            result.Data.FileName,
            enableRangeProcessing: true);
    }

    /// <summary>
    /// Update video metadata (title and description)
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <param name="request">Update request with new title and description</param>
    /// <returns>Updated video information</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 400)]
    [ProducesResponseType(typeof(ApiResponseDto<VideoResponseDto>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponseDto<VideoResponseDto>>> UpdateVideo(int id, [FromBody] VideoUploadRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value?.Errors?.Select(e => e.ErrorMessage) ?? Array.Empty<string>()).ToList();
            return BadRequest(new ApiResponseDto<VideoResponseDto>
            {
                Success = false,
                Message = "Validation failed",
                Errors = errors
            });
        }

        var result = await _videoService.UpdateVideoAsync(id, request);
        
        if (!result.Success)
        {
            return result.Message == "Video not found" ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Delete a video
    /// </summary>
    /// <param name="id">Video ID</param>
    /// <returns>Deletion result</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 200)]
    [ProducesResponseType(typeof(ApiResponseDto<bool>), 404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteVideo(int id)
    {
        var result = await _videoService.DeleteVideoAsync(id);
        
        if (!result.Success)
        {
            return result.Message == "Video not found" ? NotFound(result) : BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    /// <returns>API status</returns>
    [HttpGet("health")]
    [ProducesResponseType(200)]
    public IActionResult HealthCheck()
    {
        return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
    }
}
