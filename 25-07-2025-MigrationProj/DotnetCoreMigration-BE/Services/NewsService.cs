using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Interfaces;
using DotnetCoreMigration.Models;
using DotnetCoreMigration.Misc.Mappers;

namespace DotnetCoreMigration.Services;

public class NewsService : INewsService
{
    private readonly IRepository<News, int> _newsRepository;

    public NewsService(IRepository<News, int> newsRepository)
    {
        _newsRepository = newsRepository;
    }

    public async Task<IEnumerable<NewsDto>> GetAllNewsAsync()
    {
        var news = await _newsRepository.GetAll();
        return NewsMapper.ToDtoList(news);
    }

    public async Task<PaginatedResponse<NewsDto>> GetAllNewsPaginatedAsync(PaginationRequest request)
    {
        var paginatedNews = await _newsRepository.GetAllPaginated(request);
        var newsDtos = NewsMapper.ToDtoList(paginatedNews.Data);

        return new PaginatedResponse<NewsDto>(
            newsDtos.ToList(),
            paginatedNews.TotalCount,
            paginatedNews.PageNumber,
            paginatedNews.PageSize
        );
    }

    public async Task<NewsDto?> GetNewsByIdAsync(int id)
    {
        var news = await _newsRepository.GetById(id);
        return news == null ? null : NewsMapper.ToDto(news);
    }

    public async Task<NewsDto> CreateNewsAsync(CreateNewsDto createNewsDto)
    {
        var news = NewsMapper.ToEntity(createNewsDto);
        var createdNews = await _newsRepository.Create(news);
        return NewsMapper.ToDto(createdNews);
    }

    public async Task<NewsDto> UpdateNewsAsync(UpdateNewsDto updateNewsDto)
    {
        var existingNews = await _newsRepository.GetById(updateNewsDto.NewsId);
        if (existingNews == null)
        {
            throw new InvalidOperationException("News not found.");
        }

        NewsMapper.UpdateEntity(existingNews, updateNewsDto);
        var updatedNews = await _newsRepository.Update(existingNews);
        return NewsMapper.ToDto(updatedNews);
    }

    public async Task<bool> DeleteNewsAsync(int id)
    {
        return await _newsRepository.Delete(id);
    }

    public async Task<bool> NewsExistsAsync(int id)
    {
        var news = await _newsRepository.GetById(id);
        return news != null;
    }

    public async Task<IEnumerable<NewsDto>> GetPublishedNewsAsync()
    {
        var allNews = await _newsRepository.GetAll();
        var publishedNews = allNews.Where(n => n.IsPublished);
        return NewsMapper.ToDtoList(publishedNews);
    }

    public async Task<IEnumerable<NewsDto>> GetNewsByAuthorAsync(int authorId)
    {
        var allNews = await _newsRepository.GetAll();
        var authorNews = allNews.Where(n => n.AuthorId == authorId);
        return NewsMapper.ToDtoList(authorNews);
    }

    public async Task<bool> PublishNewsAsync(int id)
    {
        var news = await _newsRepository.GetById(id);
        if (news == null) return false;

        news.IsPublished = true;
        await _newsRepository.Update(news);
        return true;
    }

    public async Task<bool> UnpublishNewsAsync(int id)
    {
        var news = await _newsRepository.GetById(id);
        if (news == null) return false;

        news.IsPublished = false;
        await _newsRepository.Update(news);
        return true;
    }
}
