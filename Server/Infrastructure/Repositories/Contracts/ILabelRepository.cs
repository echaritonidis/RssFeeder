using RssFeeder.Server.Infrastructure.Dto;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ILabelRepository
{
    public Task<bool> RemoveLabelsByFeedId(Guid feedId, CancellationToken cancellationToken);
    public Task<bool> InsertLabelsAsync(Guid feedId, List<LabelDto> labels, CancellationToken cancellationToken);
}