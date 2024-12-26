using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface IMartenRepository<TEntity>
{
    public Task Commit(CancellationToken cancellationToken);
    public Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken);
    public Task<bool> InsertManyAsync(List<TEntity> objs, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity obj, CancellationToken cancellationToken);
    public Task UpdateRangeAsync(List<TEntity> obj, CancellationToken cancellationToken);
    public Task<bool> DeleteById(Guid entityId, CancellationToken cancellationToken);
    public Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken);
    public Task<TEntity?> GetOnePredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<IReadOnlyList<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    public Task<IReadOnlyList<TEntity>> GetAllByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
}