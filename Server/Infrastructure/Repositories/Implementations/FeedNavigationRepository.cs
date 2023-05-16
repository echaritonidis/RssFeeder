using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations
{
	public class FeedNavigationRepository : IFeedNavigationRepository
	{
        private readonly ISQLiteRepository<Feed> _repository;

        public FeedNavigationRepository(ISQLiteRepository<Feed> repository)
        {
            _repository = repository;
        }

        public async Task<List<FeedDto>> GetAllFeeds(CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllWithRelatedDataAsync(noTracking: true, cancellationToken, o => o.Labels);

            return items.Select(x => new FeedDto
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
            var model = new Feed
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                Title = feed.Title,
                Href = feed.Href,
                Default = feed.Default,
                Favorite = feed.Favorite,
                Labels = feed.Labels?.Select(label => new Label
                {
                    Id = Guid.NewGuid(),
                    Name = label.Name,
                    Color = label.Color
                }).ToList() ?? new()
            };
            
            return await _repository.InsertAsync(model, cancellationToken);
        }

        public async Task UpdateFeed(FeedDto feed, CancellationToken cancellationToken)
        {
            var item = await _repository.GetByIdWithRelatedDataAsync(feed.Id, noTracking: true, cancellationToken, o => o.Labels);

            if (item is null) return;

            item = item with
            {
                ModifiedAt = DateTime.UtcNow,
                Title = feed.Title,
                Href = feed.Href,
                Default = feed.Default,
                Favorite = feed.Favorite,
                Labels = feed.Labels.Select(label => new Label
                {
                    Id = label.Id,
                    Name = label.Name,
                    Color = label.Color
                }).ToList()
            };
            
            await _repository.UpdateAsync(item, cancellationToken);
        }

        public async Task<bool> ResetFeedDefault(List<Guid> ids, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllByIdsAsync(ids, noTracking: true, cancellationToken);
            var updateItems = items.Select(i => i with { Default = false }).ToList();
            return ids.Count == (await _repository.UpdateRangeAsync(updateItems, cancellationToken));
        }

        public Task<bool> DeleteFeed(Guid feedId, CancellationToken cancellationToken)
        {
            return _repository.DeleteById(feedId, cancellationToken);
        }
    }
}

