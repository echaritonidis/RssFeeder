using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RssFeeder.Server.Infrastructure.Logging;
using RssFeeder.Server.Infrastructure.Services.Contracts;
using RssFeeder.Shared.Model;

namespace RssFeeder.Server.Controllers.v1;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class FeedNavigationController : ControllerBase
{
    private readonly ILogger<FeedNavigationController> _logger;
    private readonly IFeedNavigationService _feedNavigationService;
    
    public FeedNavigationController
    (
        ILogger<FeedNavigationController> logger,
        IFeedNavigationService feedNavigationService
    )
    {
        _logger = logger;
        _feedNavigationService = feedNavigationService;
    }
    
    [OutputCache(Duration = 3600)]
    [HttpGet("GetFeeds")]
    public async Task<IActionResult> GetFeeds(CancellationToken cancellationToken = default)
    {
        var items = await _feedNavigationService.GetAllFeeds(cancellationToken);

        _logger.LogGetAll(items.Count);

        return Ok(items);
    }

    [OutputCache(Duration = 900, VaryByQueryKeys = new[] { "href" })]
    [HttpGet("GetContent")]
    public async Task<IActionResult> GetContent(string href, CancellationToken cancellationToken = default)
    {
        var oneXmlContentOf = await _feedNavigationService.GetXmlContent(href, cancellationToken);
        var urlEncoded = WebUtility.UrlEncode(href.Replace(Environment.NewLine, ""));
        
        return oneXmlContentOf.Match<IActionResult>
        (
            content =>
            {
                _logger.LogContent(urlEncoded, content.Count);
                return Ok(content);
            },
            notFoundContent =>
            {
                _logger.LogContentDoesntExistError(urlEncoded);
                return BadRequest("Content doesn't exist.");
            },
            notSuccessfulRequest =>
            {
                _logger.LogContentOtherError(notSuccessfulRequest.Message);
                return BadRequest(notSuccessfulRequest.Message);
            },
            notValidHref =>
            {
                _logger.LogContentOtherError(notValidHref.ErrorMessage);
                return BadRequest(notValidHref);
            }
        );
    }

    [HttpPost]
    public async Task<IActionResult> Create(FeedNavigation newFeedNavigation, CancellationToken cancellationToken = default)
    {
        var oneInsertedOf = await _feedNavigationService.InsertFeed(newFeedNavigation, cancellationToken);

        return oneInsertedOf.Match<IActionResult>
        (
            id =>
            {
                _logger.LogCreated(id);
                return Ok($"Feed with Id {id.ToString()} was created successfully");
            },
            notValidFeedNavigation =>
            {
                _logger.LogCreatedError(string.Join(", ", notValidFeedNavigation.Select(x => x.ErrorMessage)));
                return BadRequest(notValidFeedNavigation);
            }
        );
    }
    
    [HttpPut("Update")]
    public async Task<IActionResult> Update(FeedNavigation feedNavigation, CancellationToken cancellationToken = default)
    {
        var oneUpdatedOf = await _feedNavigationService.UpdateFeed(feedNavigation, cancellationToken);

        return oneUpdatedOf.Match<IActionResult>
        (
            id =>
            {
                _logger.LogUpdated(id);
                return Ok($"Feed with Id {id.ToString()} was updated successfully");
            },
            notValidFeedNavigation =>
            {
                _logger.LogUpdatedError(string.Join(", ", notValidFeedNavigation.Select(x => x.ErrorMessage)));
                return BadRequest(notValidFeedNavigation);
            }
        );
    }

    [HttpPut("ResetDefault")]
    public async Task<IActionResult> ResetDefault(List<Guid> ids, CancellationToken cancellationToken = default)
    {
        var oneUpdatedOf = await _feedNavigationService.ResetDefault(ids, cancellationToken);

        return oneUpdatedOf.Match<IActionResult>
        (
            id =>
            {
                _logger.LogDefaultReset(string.Join(", ", ids).Replace(Environment.NewLine, ""));
                return Ok($"Default feed's was successfully reset");
            },
            notValidFeedNavigation =>
            {
                _logger.LogDefaultResetError(notValidFeedNavigation.ErrorMessage);
                return BadRequest(notValidFeedNavigation);
            }
        );
    }
    

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var oneDeleteOf = await _feedNavigationService.DeleteFeed(id, cancellationToken);

        return oneDeleteOf.Match<IActionResult>
        (
            success =>
            {
                if (success)
                {
                    _logger.LogDeleted(id);
                    return Ok($"Feed with Id {id.ToString()} was deleted successfully");
                }

                return Ok($"Feed with Id {id.ToString()} wasn't deleted");
            },
            exceptionOccurred =>
            {
                _logger.LogDeletedError(exceptionOccurred.Message);
                return BadRequest("Something went wrong, please contact administrator for support.");
            }
        );
    }

    //[HttpPost("ExportExcel")]
    //public string ExportExcelFeed(List<FeedContent> content)
    //{
    //    using (var stream = new MemoryStream())
    //    {
    //        var workbook = new XLWorkbook();
    //        var SheetNames = new List<string>() { "15-16", "16-17", "17-18", "18-19", "19-20" };

    //        foreach (var sheetname in SheetNames)
    //        {
    //            var worksheet = workbook.Worksheets.Add(sheetname);

    //            worksheet.Cell("A1").Value = sheetname;
    //        }

    //        workbook.SaveAs(stream);
    //        stream.Seek(0, SeekOrigin.Begin);

    //        return Convert.ToBase64String(stream.ToArray());
    //    }
    //}

    //public string BaseCreatedDirectory
    //{
    //    get
    //    {
    //        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Created");
    //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    //        return path;
    //    }
    //}
}
