using FluentValidation;
using FluentValidation.Results;
using OneOf;
using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Implementations;

public class FeedGroupService : IFeedGroupService
{
    private readonly IFeedGroupRepository _feedGroupRepository;
    private readonly IValidator<FeedNavigationGroup> _feedNavigationGroupValidator;

    public FeedGroupService
    (
        IFeedGroupRepository feedGroupRepository,
        IValidator<FeedNavigationGroup> feedNavigationGroupValidator
    )
    {
        _feedGroupRepository = feedGroupRepository;
        _feedNavigationGroupValidator = feedNavigationGroupValidator;
    }

    public async Task<List<FeedNavigationGroup>> GetGroupedFeeds(CancellationToken cancellationToken)
    {
        var items = await _feedGroupRepository.GetGroupFeeds(cancellationToken);

        return items.Select(x => new FeedNavigationGroup
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            Color = x.Color,
            Order = x.Order,
            FeedNavigations = x.Feeds.Select(f => new FeedNavigation
            {
                Id = f.Id,
                Href = f.Href,
                Title = f.Title,
                FeedLabels = f.Labels?.Select(label => new FeedLabel
                {
                    Id = label.Id,
                    Name = label.Name,
                    Color = label.Color
                }).ToList() ?? new List<FeedLabel>(),
                Active = f.Default,
                Favorite = f.Favorite,
                Default = f.Default
            }).ToList()
        }).ToList();
    }

    public async Task<OneOf<Guid, List<ValidationFailure>>> InsertGroup(FeedNavigationGroup newFeedGroup, CancellationToken cancellationToken)
    {
        var validationResult = await _feedNavigationGroupValidator.ValidateAsync(newFeedGroup, cancellationToken);

        if (!validationResult.IsValid) return validationResult.Errors;

        return await _feedGroupRepository.InsertFeedGroup(new FeedGroupDto
        {
            Id = newFeedGroup.Id,
            Title = newFeedGroup.Title,
            Description = newFeedGroup.Description,
            Color = newFeedGroup.Color,
            Order = newFeedGroup.Order
        }, cancellationToken);
    }
    
    public async Task<OneOf<Guid, List<ValidationFailure>>> UpdateGroup(FeedNavigationGroup feedGroup, CancellationToken cancellationToken)
    {
        var validationResult = await _feedNavigationGroupValidator.ValidateAsync(feedGroup, cancellationToken);

        if (!validationResult.IsValid) return validationResult.Errors;
        
        await _feedGroupRepository.UpdateFeedGroup(new FeedGroupDto
        {
            Id = feedGroup.Id,
            Title = feedGroup.Title,
            Description = feedGroup.Description,
            Color = feedGroup.Color,
            Order = feedGroup.Order
        }, cancellationToken);

        return feedGroup.Id;
    }

    public async Task<OneOf<bool, Exception>> DeleteGroup(Guid feedGroupId, CancellationToken cancellationToken)
    {
        try
        {
            return await _feedGroupRepository.DeleteFeedGroup(feedGroupId, cancellationToken);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}