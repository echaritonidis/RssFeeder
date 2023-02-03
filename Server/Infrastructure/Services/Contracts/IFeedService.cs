using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts;

public interface IFeedService 
{
    public Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken);

    public Task InsertFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);
}