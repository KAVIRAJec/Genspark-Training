using DotnetCoreMigration.DTOs;

namespace DotnetCoreMigration.Interfaces;

public interface INewsService
{
    Task<IEnumerable<NewsDto>> GetAllNewsAsync();
    Task<PaginatedResponse<NewsDto>> GetAllNewsPaginatedAsync(PaginationRequest request);
    Task<NewsDto?> GetNewsByIdAsync(int id);
    Task<NewsDto> CreateNewsAsync(CreateNewsDto createNewsDto);
    Task<NewsDto> UpdateNewsAsync(UpdateNewsDto updateNewsDto);
    Task<bool> DeleteNewsAsync(int id);
    Task<bool> NewsExistsAsync(int id);
    Task<IEnumerable<NewsDto>> GetPublishedNewsAsync();
    Task<IEnumerable<NewsDto>> GetNewsByAuthorAsync(int authorId);
    Task<bool> PublishNewsAsync(int id);
    Task<bool> UnpublishNewsAsync(int id);
}
