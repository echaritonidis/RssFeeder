using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations;

public class FeedNavigationGroupRepository : IFeedNavigationGroupRepository
{
    private readonly ISQLiteRepository<FeedGroup> _repository;

    public FeedNavigationGroupRepository(ISQLiteRepository<FeedGroup> repository)
    {
        _repository = repository;
    }
    
    public async Task<List<FeedGroupDto>> GetGroupNames(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllWithRelatedDataAsync(noTracking: true, cancellationToken);

        return items.Select(g => new FeedGroupDto
        {
            Id = g.Id,
            Title = g.Title,
            Initial = g.Initial
        }).ToList();
    }

    public async Task<List<FeedGroupDto>> GetGroupFeeds(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllWithRelatedDataAsync(noTracking: true, cancellationToken, o => o.Feeds);

        return items.Select(g => new FeedGroupDto
        {
            Id = g.Id,
            Title = g.Title,
            Description = g.Description,
            Color = g.Color,
            Order = g.Order,
            Initial = g.Initial,
            Feeds = g.Feeds.Select(x => new FeedDto
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
                }).ToList() ?? new List<LabelDto>()
            }).ToList()
        }).ToList();
    }

    public async Task<Guid> InsertFeedGroup(FeedGroupDto feedGroup, CancellationToken cancellationToken)
    {
        var model = new FeedGroup
        {
            Id = Guid.NewGuid(),
            Title = feedGroup.Title,
            Description = feedGroup.Description,
            Order = feedGroup.Order,
            Initial = false,
            Color = feedGroup.Color
        };
            
        return await _repository.InsertAsync(model, cancellationToken);
    }
   
    public async Task UpdateFeedGroup(FeedGroupDto feedGroup, CancellationToken cancellationToken)
    {
        var item = await _repository.GetByIdWithRelatedDataAsync(feedGroup.Id, noTracking: true, cancellationToken, o => o.Feeds);

        if (item is null) return;
                
        item = item with
        {
            Title = feedGroup.Title,
            Description = feedGroup.Description,
            Color = feedGroup.Color,
            Order = feedGroup.Order,
            Initial = feedGroup.Initial
        };
            
        await _repository.UpdateAsync(item, cancellationToken);
    }

    public Task<bool> DeleteFeedGroup(Guid feedGroupId, CancellationToken cancellationToken)
    {
        return _repository.DeleteById(feedGroupId, cancellationToken);
    }
}