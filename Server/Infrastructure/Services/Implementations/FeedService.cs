using FluentValidation;
using FluentValidation.Results;
using OneOf;
using OneOf.Types;
using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Exceptions;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Infrastructure.Services.Implementations;

public class FeedService : IFeedService
{
    private readonly IFeedRepository _feedRepository;
    private readonly ILabelRepository _labelRepository;
    private readonly IExtractContent _extractContent;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IValidator<FeedNavigation> _feedValidator;

    public FeedService
    (
        IFeedRepository feedRepository,
        ILabelRepository labelRepository,
        IExtractContent extractContent,
        IHttpClientFactory httpClientFactory,
        IValidator<FeedNavigation> feedValidator
    )
    {
        _feedRepository = feedRepository;
        _labelRepository = labelRepository;
        _extractContent = extractContent;
        _httpClientFactory = httpClientFactory;
        _feedValidator = feedValidator;
    }

    public async Task<List<FeedNavigation>> GetAllFeeds(CancellationToken cancellationToken)
    {
        var items = await _feedRepository.GetAllFeeds(cancellationToken);

        return items.Select(x => new FeedNavigation
        {
            Id = x.Id,
            Href = x.Href,
            Title = x.Title,
            FeedLabels = x.Labels?.Select(label => new FeedLabel
            {
                Id = label.Id,
                Name = label.Name,
                Color = label.Color
            }).ToList() ?? new(),
            Active = x.Default,
            Favorite = x.Favorite,
            Default = x.Default
        }).ToList();
    }

    public async Task<OneOf<Guid, List<ValidationFailure>>> InsertFeed(FeedNavigation newFeedNavigation, CancellationToken cancellationToken)
    {
        var validationResult = await _feedValidator.ValidateAsync(newFeedNavigation, cancellationToken);

        if (!validationResult.IsValid) return validationResult.Errors;

        var feedId = await _feedRepository.InsertFeed(new FeedDto
        {
            GroupId = newFeedNavigation.GroupId,
            Href = newFeedNavigation.Href,
            Title = newFeedNavigation.Title,
            Favorite = newFeedNavigation.Favorite,
            Default = newFeedNavigation.Default
        }, cancellationToken);

        if (newFeedNavigation.FeedLabels?.Count > 0)
        {
            var labels = newFeedNavigation.FeedLabels.Select(label => new LabelDto
            {
                Name = label.Name,
                Color = label.Color,
            }).ToList();

            await _labelRepository.InsertLabelsAsync(feedId, labels, cancellationToken);
        }

        return feedId;
    }

    public async Task<OneOf<Guid, List<ValidationFailure>>> UpdateFeed(FeedNavigation feedNavigation, CancellationToken cancellationToken)
    {
        var validationResult = await _feedValidator.ValidateAsync(feedNavigation, cancellationToken);

        if (!validationResult.IsValid) return validationResult.Errors;

        await _labelRepository.RemoveLabelsByFeedId(feedNavigation.Id, cancellationToken);
        
        await _feedRepository.UpdateFeed(new FeedDto
        {
            Id = feedNavigation.Id,
            Href = feedNavigation.Href,
            Title = feedNavigation.Title,
            Favorite = feedNavigation.Favorite,
            Default = feedNavigation.Default,
            ModifiedAt = DateTime.UtcNow
        }, cancellationToken);
        
        if (feedNavigation.FeedLabels?.Count > 0)
        {
            var labels = feedNavigation.FeedLabels.Select(label => new LabelDto
            {
                Name = label.Name,
                Color = label.Color,
            }).ToList();

            await _labelRepository.InsertLabelsAsync(feedNavigation.Id, labels, cancellationToken);
        }
        
        return feedNavigation.Id;
    }

    public async Task<OneOf<bool, ValidationFailure>> ResetDefault(List<Guid> ids, CancellationToken cancellationToken)
    {
        if (ids is null || ids.Count == 0) return new ValidationFailure("Ids", "Please provide Id's to reset.");

        return await _feedRepository.ResetFeedDefault(ids, cancellationToken);
    }

    public async Task<OneOf<bool, Exception>> DeleteFeed(Guid feedId, CancellationToken cancellationToken)
    {
        try
        {
            return await _feedRepository.DeleteFeed(feedId, cancellationToken);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<OneOf<List<FeedContent>, NotFound, CustomHttpRequestException, ValidationFailure>> GetXmlContent(string href, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(href)) return new ValidationFailure("Href", "Href should not be empty.");

        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(href, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new CustomHttpRequestException(response.StatusCode, response.ReasonPhrase);
        }

        var xmlContent = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(xmlContent))
        {
            return new NotFound();
        }

        return await _extractContent.GetContentItems(xmlContent);
    }
}