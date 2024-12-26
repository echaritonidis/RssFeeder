using FluentValidation.Results;
using OneOf;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts;

public interface IFeedGroupService
{
    public Task<List<FeedNavigationGroupNames>> GetGroupedNames(CancellationToken cancellationToken);
    public Task<List<FeedNavigationGroup>> GetGroupedFeeds(CancellationToken cancellationToken);
    public Task<OneOf<Guid, List<ValidationFailure>>> InsertGroup(FeedNavigationGroup newFeedGroup, CancellationToken cancellationToken);
    public Task<OneOf<Guid, List<ValidationFailure>>> UpdateGroup(FeedNavigationGroup feedGroup, CancellationToken cancellationToken);
    public Task<OneOf<bool, Exception>> DeleteGroup(Guid feedGroupId, CancellationToken cancellationToken);
}