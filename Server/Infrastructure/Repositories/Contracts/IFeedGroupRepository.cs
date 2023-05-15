using RssFeeder.Server.Infrastructure.Dto;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface IFeedGroupRepository
{
    public Task<List<FeedGroupDto>> GetGroupFeeds(CancellationToken cancellationToken);
    public Task<Guid> InsertFeedGroup(FeedGroupDto feedGroup, CancellationToken cancellationToken);
    public Task<Guid> InsertFeed(FeedDto feed, CancellationToken cancellationToken);
    public Task UpdateFeedGroup(FeedGroupDto feedGroup, CancellationToken cancellationToken);
    public Task<bool> DeleteFeedGroup(Guid feedGroupId, CancellationToken cancellationToken);
}