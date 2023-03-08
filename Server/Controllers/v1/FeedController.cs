using Asp.Versioning;
using ClosedXML.Excel;
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

    [HttpPut("Update")]
    public Task Update(FeedNavigation newFeedNavigation, CancellationToken cancellationToken = default)
    {
        return _feedService.UpdateFeed(newFeedNavigation, cancellationToken);
    }

    [HttpPost("ExportExcel")]
    public string ExportExcelFeed(List<FeedContent> content)
    {
        using (var stream = new MemoryStream())
        {
            var workbook = new XLWorkbook();
            var SheetNames = new List<string>() { "15-16", "16-17", "17-18", "18-19", "19-20" };

            foreach (var sheetname in SheetNames)
            {
                var worksheet = workbook.Worksheets.Add(sheetname);

                worksheet.Cell("A1").Value = sheetname;
            }

            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return Convert.ToBase64String(stream.ToArray());
        }
    }

    public string BaseCreatedDirectory
    {
        get
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Created");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }
}
