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
public class FeedNavigationGroupController : ControllerBase
{
    private readonly ILogger<FeedNavigationGroupController> _logger;
    private readonly IFeedGroupService _feedGroupService;
    
    public FeedNavigationGroupController
    (
        ILogger<FeedNavigationGroupController> logger,
        IFeedGroupService feedGroupService
    )
    {
        _logger = logger;
        _feedGroupService = feedGroupService;
    }

    [OutputCache(Duration = 3600)]
    [HttpGet("GetFeedGroups")]
    public async Task<IActionResult> GetFeedGroups(CancellationToken cancellationToken = default)
    {
        var items = await _feedGroupService.GetGroupedFeeds(cancellationToken);

        _logger.LogGetAll(items.Count);

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(FeedNavigationGroup newFeedNavigationGroup, CancellationToken cancellationToken = default)
    {
        var oneInsertedOf = await _feedGroupService.InsertGroup(newFeedNavigationGroup, cancellationToken);

        return oneInsertedOf.Match<IActionResult>
        (
            id =>
            {
                _logger.LogCreated(id);
                return Ok($"Feed Group with Id {id.ToString()} was created successfully");
            },
            notValidFeedNavigation =>
            {
                _logger.LogCreatedError(string.Join(", ", notValidFeedNavigation.Select(x => x.ErrorMessage)));
                return BadRequest(notValidFeedNavigation);
            }
        );
    }

    [HttpPut("Update")]
    public async Task<IActionResult> Update(FeedNavigationGroup feedNavigationGroup, CancellationToken cancellationToken = default)
    {
        var oneUpdatedOf = await _feedGroupService.UpdateGroup(feedNavigationGroup, cancellationToken);

        return oneUpdatedOf.Match<IActionResult>
        (
            id =>
            {
                _logger.LogUpdated(id);
                return Ok($"Feed Group with Id {id.ToString()} was updated successfully");
            },
            notValidFeedNavigation =>
            {
                _logger.LogUpdatedError(string.Join(", ", notValidFeedNavigation.Select(x => x.ErrorMessage)));
                return BadRequest(notValidFeedNavigation);
            }
        );
    }

    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var oneDeleteOf = await _feedGroupService.DeleteGroup(id, cancellationToken);

        return oneDeleteOf.Match<IActionResult>
        (
            success =>
            {
                if (success)
                {
                    _logger.LogDeleted(id);
                    return Ok($"Feed Group with Id {id.ToString()} was deleted successfully");
                }

                return Ok($"Feed Group with Id {id.ToString()} wasn't deleted");
            },
            exceptionOccurred =>
            {
                _logger.LogDeletedError(exceptionOccurred.Message);
                return BadRequest("Something went wrong, please contact administrator for support.");
            }
        );
    }
}
