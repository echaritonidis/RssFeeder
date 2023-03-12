using FluentValidation;
using OneOf;
using OneOf.Types;
using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Exceptions;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;
using System.Net;

namespace RssFeeder.Server.Infrastructure.Services.Implementations;

public class FeedService : IFeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly IExtractContent _extractContent;
    private readonly IHttpClientFactory _httpClientFactory;

    public FeedService
    (
        IFeedRepository feedRepository,
        IExtractContent extractContent,
        IHttpClientFactory httpClientFactory
    )
    {
        _feedRepository = feedRepository;
        _extractContent = extractContent;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken)
    {
        var items = await _feedRepository.GetAllFeeds(cancellationToken);

        return items.Select(x => new FeedNavigation
        {
            Id = x.Id,
            Href = x.Href,
            Title = x.Title,
            Tags = x.Tags?.Select(tag => new FeedTag
            {
                Name = tag.Name,
                Color = tag.Color
            }).ToList() ?? new(),
            Favorite = x.Favorite,
            Default = x.Default
        }).ToList();
    }

    public async Task<Guid> InsertFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken)
    {
        return await _feedRepository.InsertFeed(new FeedDto
        {
            Id = feedNavigation.Id,
            Href = feedNavigation.Href,
            Title = feedNavigation.Title,
            Tags = feedNavigation.Tags?.Select(tag => new TagsDto
            {
                Name = tag.Name,
                Color = tag.Color
            }).ToList() ?? new(),
            Favorite = feedNavigation.Favorite,
            Default = feedNavigation.Default
        }, cancellationToken);
    }

    public async Task UpdateFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken)
    {
        await _feedRepository.UpdateFeed(new FeedDto
        {
            Id = feedNavigation.Id,
            Href = feedNavigation.Href,
            Title = feedNavigation.Title,
            Tags = feedNavigation.Tags?.Select(tag => new TagsDto
            {
                Name = tag.Name,
                Color = tag.Color
            }).ToList() ?? new(),
            Favorite = feedNavigation.Favorite,
            Default = feedNavigation.Default
        }, cancellationToken);
    }

    public async Task<OneOf<List<FeedContent>, NotFound, CustomHttpRequestException, ValidationException>> GetXmlContent(string href, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(href)) return new ValidationException("Href should not be empty.");

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(href, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new CustomHttpRequestException(response.StatusCode, response.ReasonPhrase);
        }

        var xmlContent = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(xmlContent))
        {
            return new NotFound();
        }

        return _extractContent.GetContentItems(xmlContent);
    }
}