using RssFeeder.Server.Infrastructure.Dto;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations
{
	public class LabelRepository : ILabelRepository
	{
        private readonly IMartenRepository<Label> _repository;

        public LabelRepository(IMartenRepository<Label> repository)
        {
            _repository = repository;
        }

        public async Task<bool> RemoveLabelsByFeedId(Guid feedId, CancellationToken cancellationToken)
        {
            var labels = await _repository.GetAllPredicatedAsync(x => x.FeedId == feedId, cancellationToken);

            if (labels.Count == 0) return true;
            
            return await _repository.DeleteByIds(labels.Select(x => x.Id).ToList(), cancellationToken);
        }

        public async Task<bool> InsertLabelsAsync(Guid newFeedId, List<LabelDto> labels, CancellationToken cancellationToken)
        {
            return await _repository.InsertManyAsync(labels.Select(label => new Label
            {
                Id = Guid.NewGuid(),
                Name = label.Name,
                Color = label.Color,
                FeedId = newFeedId
            }).ToList(), cancellationToken);
        }
    }
}

