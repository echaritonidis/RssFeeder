namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ILabelRepository
{
    public Task<bool> RemoveLabelsByFeedId(Guid feedId, List<Guid> labelIdsToExclude, CancellationToken cancellationToken);
}