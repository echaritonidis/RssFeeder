using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations
{
	public class LabelRepository : ILabelRepository
	{
        private readonly ISQLiteRepository<Label> _repository;

        public LabelRepository(ISQLiteRepository<Label> repository)
        {
            _repository = repository;
        }

        public async Task<bool> RemoveLabelsByFeedId(Guid feedId, List<Guid> labelIdsToExclude, CancellationToken cancellationToken)
        {
            var labels = await _repository.GetAllPredicatedAsync(x => x.FeedId == feedId && !labelIdsToExclude.Contains(x.Id), noTracking: true, cancellationToken);

            if (labels.Count == 0) return true;
            
            return await _repository.DeleteByIds(labels.Select(x => x.Id).ToList(), cancellationToken);
        }
    }
}

