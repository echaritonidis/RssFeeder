using RssFeeder.Server.Infrastructure.Dto;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts
{
    public interface IFeedRepository
    {
        public Task<List<FeedDto>> GetAllFeeds(CancellationToken cancellationToken);
        public Task<Guid> InsertFeed(FeedDto feed, CancellationToken cancellationToken);
        public Task UpdateFeed(FeedDto feed, CancellationToken cancellationToken);
        public Task<bool> ResetFeedDefault(List<Guid> ids, CancellationToken cancellationToken);
        public Task<bool> DeleteFeed(Guid feedId, CancellationToken cancellationToken);
    }
}

