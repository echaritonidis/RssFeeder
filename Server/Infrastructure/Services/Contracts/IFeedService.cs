using FluentValidation;
using OneOf;
using OneOf.Types;
using RssFeeder.Server.Infrastructure.Exceptions;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Contracts;

public interface IFeedService 
{
    public Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken);

    public Task<Guid> InsertFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);

    public Task UpdateFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken);

    public Task<OneOf<List<FeedContent>, NotFound, CustomHttpRequestException, ValidationException>> GetXmlContent(string href, CancellationToken cancellationToken);
}