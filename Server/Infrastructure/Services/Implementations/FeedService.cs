using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Implementations;

public class FeedService : IFeedService
{
    private readonly IFeedRepository _feedRepository;

    public FeedService(IFeedRepository feedRepository)
    {
        _feedRepository = feedRepository;
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

    public async Task InsertFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken)
    {
        await _feedRepository.InsertFeed(new FeedDto
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
}