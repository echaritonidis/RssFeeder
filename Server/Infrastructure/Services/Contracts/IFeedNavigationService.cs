using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using RssFeeder.Server.Infrastructure.Exceptions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts;

public interface IFeedNavigationService 
{
    public Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken);
    public Task<OneOf<Guid, List<ValidationFailure>>> InsertFeed(FeedNavigation newFeedNavigation, CancellationToken cancellationToken);
    public Task<OneOf<Guid, List<ValidationFailure>>> UpdateFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);
    public Task<OneOf<bool, ValidationFailure>> ResetDefault(List<Guid> ids, CancellationToken cancellationToken);
    public Task<OneOf<bool, Exception>> DeleteFeed(Guid feedId, CancellationToken cancellationToken);
    public Task<OneOf<List<FeedContent>, NotFound, CustomHttpRequestException, ValidationFailure>> GetXmlContent(string href, CancellationToken cancellationToken);
}