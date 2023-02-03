using System.Globalization;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Server.Infrastructure.Services.Implementations;
using RssFeeder.Shared;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Controllers;

[ApiController]
[Route("[controller]")]
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

    [HttpGet("GetAll")]
    public Task<List<FeedNavigation>> GetAll(CancellationToken cancellationToken = default)
    {
        return _feedService.GetAllFeeds(cancellationToken);
    }

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
