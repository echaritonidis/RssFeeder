using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ISQLiteRepository<TEntity>
{
    public Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity obj, CancellationToken cancellationToken);
    public Task<int> UpdateRangeAsync(List<TEntity> obj, CancellationToken cancellationToken);
    public Task<bool> DeleteById(Guid entityId, CancellationToken cancellationToken);
    public Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties);
    public Task<TEntity?> GetByIdWithRelatedDataAsync(Guid entityId, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties);
}