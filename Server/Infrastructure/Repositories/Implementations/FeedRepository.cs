using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations
{
	public class FeedRepository : IFeedRepository
	{
        private readonly ISQLiteRepository<Feed> _repository;

        public FeedRepository(ISQLiteRepository<Feed> repository)
        {
            _repository = repository;
        }

        public async Task<List<FeedDto>> GetAllFeeds(CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync(cancellationToken);

            return items.Select(x => new FeedDto
            {
                Id = x.Id,
                Title = x.Title,
                Href = x.Href,
                Default = x.Default,
                Favorite = x.Favorite,
                Tags = x.Tags?.Select(tag => new TagsDto
                {
                    Name = tag.Name,
                    Color = tag.Color
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
                Tags = feed.Tags?.Select(tag => new Tags
                {
                    Id = Guid.NewGuid(),
                    Name = tag.Name,
                    Color = tag.Color
                }).ToList() ?? new()
            };
            
            return await _repository.InsertAsync(model, cancellationToken);
        }

        public async Task UpdateFeed(FeedDto feed, CancellationToken cancellationToken)
        {
            var item = await _repository.GetAsync(feed.Id, cancellationToken);

            if (item is null) return;

            item = item with
            {
                ModifiedAt = DateTime.UtcNow,
                Title = feed.Title,
                Href = feed.Href,
                Default = feed.Default,
                Favorite = feed.Favorite,
                Tags = feed.Tags.Select(tag => new Tags
                {
                    Id = Guid.NewGuid(),
                    Name = tag.Name,
                    Color = tag.Color
                }).ToList()
            };
            
            await _repository.UpdateAsync(item, cancellationToken);
        }
    }
}

