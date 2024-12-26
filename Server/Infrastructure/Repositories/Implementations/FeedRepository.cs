using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations
{
	public class FeedRepository : IFeedRepository
	{
        private readonly IMartenRepository<Feed> _repository;

        public FeedRepository(IMartenRepository<Feed> repository)
        {
            _repository = repository;
        }

        public async Task<List<FeedDto>> GetAllFeeds(CancellationToken cancellationToken)
        {
            var feeds = await _repository.GetAllAsync(cancellationToken);

            return feeds.Select(x => new FeedDto
            {
                Id = x.Id,
                Title = x.Title,
                Href = x.Href,
                Default = x.Default,
                Favorite = x.Favorite,
                Labels = x.Labels?.Select(label => new LabelDto
                {
                    Id = label.Id,
                    Name = label.Name,
                    Color = label.Color
                }).ToList() ?? new()
            }).ToList();
        }

        public async Task<Guid> InsertFeed(FeedDto feed, CancellationToken cancellationToken)
        {
            var newFeedId = Guid.NewGuid();
            
            var model = new Feed
            {
                Id = newFeedId,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                Title = feed.Title,
                Href = feed.Href,
                Default = feed.Default,
                Favorite = feed.Favorite,
                FeedGroupId = feed.GroupId
            };

            return await _repository.InsertAsync(model, cancellationToken);
        }

        public async Task UpdateFeed(FeedDto feed, CancellationToken cancellationToken)
        {
            var item = await _repository.GetOnePredicatedAsync(x => x.Id == feed.Id, cancellationToken);

            if (item is null) return;

            item = item with
            {
                ModifiedAt = DateTime.UtcNow,
                Title = feed.Title,
                Href = feed.Href,
                Default = feed.Default,
                Favorite = feed.Favorite
            };
            
            await _repository.UpdateAsync(item, cancellationToken);
        }

        public async Task<bool> ResetFeedDefault(List<Guid> ids, CancellationToken cancellationToken)
        {
            try
            {
                var items = await _repository.GetAllByIdsAsync(ids, cancellationToken);
                var updateItems = items.Select(i => i with { Default = false }).ToList();
                await _repository.UpdateRangeAsync(updateItems, cancellationToken);

                return true;
            }
            catch 
            {
                return false;
            }
        }

        public Task<bool> DeleteFeed(Guid feedId, CancellationToken cancellationToken)
        {
            return _repository.DeleteById(feedId, cancellationToken);
        }
    }
}

