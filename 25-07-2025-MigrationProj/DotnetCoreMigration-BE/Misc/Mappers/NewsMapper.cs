using DotnetCoreMigration.DTOs;
using DotnetCoreMigration.Models;

namespace DotnetCoreMigration.Misc.Mappers;

public static class NewsMapper
{
    public static NewsDto ToDto(News news)
    {
        return new NewsDto
        {
            NewsId = news.NewsId,
            Title = news.Title,
            Content = news.Content,
            Summary = news.Summary,
            Image = news.Image,
            CreatedDate = news.CreatedDate,
            UpdatedDate = news.UpdatedDate,
            IsPublished = news.IsPublished,
            IsActive = news.IsActive,
            AuthorId = news.AuthorId ?? 0,
            AuthorName = news.Author?.UserName ?? "Unknown"
        };
    }

    public static News ToEntity(CreateNewsDto createDto)
    {
        return new News
        {
            Title = createDto.Title,
            Content = createDto.Content,
            Summary = createDto.Summary,
            Image = createDto.Image,
            IsPublished = createDto.IsPublished,
            AuthorId = createDto.AuthorId,
            IsActive = true
        };
    }

    public static void UpdateEntity(News entity, UpdateNewsDto updateDto)
    {
        entity.Title = updateDto.Title;
        entity.Content = updateDto.Content;
        entity.Summary = updateDto.Summary;
        entity.Image = updateDto.Image;
        entity.IsPublished = updateDto.IsPublished;
        entity.AuthorId = updateDto.AuthorId;
        entity.UpdatedDate = DateTime.UtcNow;
    }

    public static IEnumerable<NewsDto> ToDtoList(IEnumerable<News> newsList)
    {
        return newsList.Select(ToDto);
    }
}
