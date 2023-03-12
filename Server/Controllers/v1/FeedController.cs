using Asp.Versioning;
using ClosedXML.Excel;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using OneOf;
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
    

    private readonly IValidator<FeedNavigation> _feedNavigationValidator;

    public FeedController
    (
        ILogger<FeedController> logger,
        IFeedService feedService,
        IValidator<FeedNavigation> feedNavigationValidator
    )
    {
        _logger = logger;
        _feedService = feedService;
        _feedNavigationValidator = feedNavigationValidator;
    }

    [OutputCache(Duration = 3600)]
    [HttpGet("GetAll")]
    public Task<List<FeedNavigation>> GetAll(CancellationToken cancellationToken = default)
    {
        return _feedService.GetAllFeeds(cancellationToken);
    }

    [OutputCache(Duration = 3600, VaryByQueryKeys = new[] { "href" })]
    [HttpGet("GetContent")]
    public async Task<IActionResult> GetContent(string href, CancellationToken cancellationToken = default)
    {
        var oneXmlContentOf = await _feedService.GetXmlContent(href, cancellationToken);

        return oneXmlContentOf.Match<IActionResult>
        (
            content => Ok(content),
            notFoundContent => BadRequest("Content doesn't exist."),
            notSuccessfulRequest => BadRequest(notSuccessfulRequest.Message),
            notValidHref => BadRequest(notValidHref.Message)
        );
    }

    [HttpPost]
    public async Task<OneOf<OkObjectResult, ValidationException>> Create(FeedNavigation newFeedNavigation, CancellationToken cancellationToken = default)
    {
        await _feedNavigationValidator.ValidateAndThrowAsync(newFeedNavigation);

        var id = await _feedService.InsertFeed(newFeedNavigation, cancellationToken);
        
        return Ok(id);
    }

    [HttpPut("Update")]
    public async Task<OneOf<OkResult, ValidationException>> Update(FeedNavigation newFeedNavigation, CancellationToken cancellationToken = default)
    {
        await _feedNavigationValidator.ValidateAndThrowAsync(newFeedNavigation);

        await _feedService.UpdateFeed(newFeedNavigation, cancellationToken);

        return Ok();
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
