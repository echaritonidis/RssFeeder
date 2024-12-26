using Marten;
using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations;

public class FeedGroupRepository : IFeedGroupRepository
{
    private readonly IMartenRepository<FeedGroup> _repository;
    private readonly IQuerySession _querySession;

    public FeedGroupRepository(IMartenRepository<FeedGroup> repository, IQuerySession querySession)
    {
        _repository = repository;
        _querySession = querySession;
    }
    
    public async Task<List<FeedGroupDto>> GetGroupNames(CancellationToken cancellationToken)
    {
        var items = await _repository.GetAllAsync(cancellationToken);

        return items.Select(g => new FeedGroupDto
        {
            Id = g.Id,
            Title = g.Title,
            Initial = g.Initial
        }).ToList();
    }

    public async Task<List<FeedGroupDto>> GetGroupFeeds(CancellationToken cancellationToken)
    {
        var feedGroups = await _repository.GetAllAsync(cancellationToken);
        var feedGroupIds = feedGroups.Select(x => x.Id).ToList();
        
        // Retrieve all Feeds
        var feeds = _querySession.Query<Feed>().Where(x => feedGroupIds.Contains(x.FeedGroupId)).ToList();
        var feedIds = feeds.Select(x => x.Id).ToList();

        // Retrieve all labels
        var labels = _querySession.Query<Label>().Where(x => feedIds.Contains(x.FeedId)).ToList();

        return feedGroups.Select(g => new FeedGroupDto
        {
            Id = g.Id,
            Title = g.Title,
            Description = g.Description,
            Color = g.Color,
            Order = g.Order,
            Initial = g.Initial,
            Feeds = feeds?.Where(f => f.FeedGroupId == g.Id).Select(x => new FeedDto
            {
                Id = x.Id,
                Title = x.Title,
                Href = x.Href,
                Default = x.Default,
                Favorite = x.Favorite,
                Labels = labels?.Where(l => l.FeedId == x.Id).Select(label => new LabelDto
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
        var item = await _repository.GetOnePredicatedAsync(x => x.Id == feedGroup.Id, cancellationToken);

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