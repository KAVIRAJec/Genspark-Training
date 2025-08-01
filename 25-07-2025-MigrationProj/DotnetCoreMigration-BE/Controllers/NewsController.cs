using Microsoft.AspNetCore.Mvc;
using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Extensions;

namespace DotnetCoreMigration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    /// <summary>
    /// Get all news (non-paginated)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<NewsDto>>>> GetAllNews()
    {
        try
        {
            var news = await _newsService.GetAllNewsAsync();
            return Ok(ApiResponse<IEnumerable<NewsDto>>.SuccessResponse(news, "News retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<NewsDto>>.ErrorResponse("An error occurred while retrieving news", ex.Message));
        }
    }

    /// <summary>
    /// Get all news with pagination
    /// </summary>
    [HttpGet("paginated")]
    public async Task<ActionResult<ApiResponse<PaginatedResponse<NewsDto>>>> GetAllNewsPaginated([FromQuery] PaginationRequest request)
    {
        try
        {
            var paginatedNews = await _newsService.GetAllNewsPaginatedAsync(request);
            return Ok(ApiResponse<PaginatedResponse<NewsDto>>.SuccessResponse(paginatedNews, "Paginated news retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<PaginatedResponse<NewsDto>>.ErrorResponse("An error occurred while retrieving news", ex.Message));
        }
    }

    /// <summary>
    /// Get news by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NewsDto>>> GetNewsById(int id)
    {
        try
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound(ApiResponse<NewsDto>.ErrorResponse("News not found", $"No news found with ID {id}"));
            }
            return Ok(ApiResponse<NewsDto>.SuccessResponse(news, "News retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NewsDto>.ErrorResponse("An error occurred while retrieving the news", ex.Message));
        }
    }

    /// <summary>
    /// Get published news only
    /// </summary>
    [HttpGet("published")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NewsDto>>>> GetPublishedNews()
    {
        try
        {
            var publishedNews = await _newsService.GetPublishedNewsAsync();
            return Ok(ApiResponse<IEnumerable<NewsDto>>.SuccessResponse(publishedNews, "Published news retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<NewsDto>>.ErrorResponse("An error occurred while retrieving published news", ex.Message));
        }
    }

    /// <summary>
    /// Get news by author
    /// </summary>
    [HttpGet("author/{authorId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NewsDto>>>> GetNewsByAuthor(int authorId)
    {
        try
        {
            var authorNews = await _newsService.GetNewsByAuthorAsync(authorId);
            return Ok(ApiResponse<IEnumerable<NewsDto>>.SuccessResponse(authorNews, "News by author retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<NewsDto>>.ErrorResponse("An error occurred while retrieving news by author", ex.Message));
        }
    }

    /// <summary>
    /// Create a new news article
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NewsDto>>> CreateNews([FromBody] CreateNewsDto createNewsDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<NewsDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var news = await _newsService.CreateNewsAsync(createNewsDto);
            return CreatedAtAction(nameof(GetNewsById), new { id = news.NewsId }, ApiResponse<NewsDto>.SuccessResponse(news, "News created successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NewsDto>.ErrorResponse("An error occurred while creating the news", ex.Message));
        }
    }

    /// <summary>
    /// Update an existing news article
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<NewsDto>>> UpdateNews(int id, [FromBody] UpdateNewsDto updateNewsDto)
    {
        try
        {
            if (id != updateNewsDto.NewsId)
            {
                return BadRequest(ApiResponse<NewsDto>.ErrorResponse("Validation failed", "ID in URL does not match ID in request body"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<NewsDto>.ErrorResponse("Validation failed", ModelState.GetErrorMessage()));
            }

            var exists = await _newsService.NewsExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<NewsDto>.ErrorResponse("News not found", $"No news found with ID {id}"));
            }

            var news = await _newsService.UpdateNewsAsync(updateNewsDto);
            return Ok(ApiResponse<NewsDto>.SuccessResponse(news, "News updated successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ApiResponse<NewsDto>.ErrorResponse("News not found", ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NewsDto>.ErrorResponse("An error occurred while updating the news", ex.Message));
        }
    }

    /// <summary>
    /// Publish a news article
    /// </summary>
    [HttpPatch("{id}/publish")]
    public async Task<ActionResult<ApiResponse<bool>>> PublishNews(int id)
    {
        try
        {
            var result = await _newsService.PublishNewsAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("News not found", $"No news found with ID {id}"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "News published successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while publishing the news", ex.Message));
        }
    }

    /// <summary>
    /// Unpublish a news article
    /// </summary>
    [HttpPatch("{id}/unpublish")]
    public async Task<ActionResult<ApiResponse<bool>>> UnpublishNews(int id)
    {
        try
        {
            var result = await _newsService.UnpublishNewsAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("News not found", $"No news found with ID {id}"));
            }
            return Ok(ApiResponse<bool>.SuccessResponse(true, "News unpublished successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while unpublishing the news", ex.Message));
        }
    }

    /// <summary>
    /// Delete a news article (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteNews(int id)
    {
        try
        {
            var exists = await _newsService.NewsExistsAsync(id);
            if (!exists)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("News not found", $"No news found with ID {id}"));
            }

            var result = await _newsService.DeleteNewsAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "News deleted successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the news", ex.Message));
        }
    }
}
