using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class FeedController : ControllerBase
{
    private readonly ILogger<FeedController> _logger;
    private readonly IFeedService _feedService;
    private readonly IExtractContent _extractContent;
    private readonly IHttpClientFactory _httpClientFactory;

    public FeedController
    (
        ILogger<FeedController> logger,
        IFeedService feedService,
        IExtractContent extractContent,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _feedService = feedService;
        _extractContent = extractContent;
        _httpClientFactory = httpClientFactory;
    }

    [OutputCache(Duration = 900)]
    [HttpGet("GetAll")]
    public Task<List<FeedNavigation>> GetAll(CancellationToken cancellationToken = default)
    {
        return _feedService.GetAllFeeds(cancellationToken);
    }

    [OutputCache(Duration = 1800)]
    [HttpGet("GetContent")]
    public async Task<List<FeedContent>> GetContent(string href, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync(href, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var xmlContent = await response.Content.ReadAsStringAsync();

            return _extractContent.GetContentItems(xmlContent);
        }

        return new();
    }

    [HttpPost]
    public Task Create(FeedNavigation newFeedNavigation, CancellationToken cancellationToken = default)
    {
        return _feedService.InsertFeed(newFeedNavigation, cancellationToken);
    }
}
