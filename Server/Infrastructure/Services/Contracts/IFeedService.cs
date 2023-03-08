using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts;

public interface IFeedService 
{
    public Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken);

    public Task InsertFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);

    public Task UpdateFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);
}