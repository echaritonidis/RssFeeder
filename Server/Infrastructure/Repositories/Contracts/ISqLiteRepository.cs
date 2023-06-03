using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ISqLiteRepository<TEntity>
{
    public Task Commit(CancellationToken cancellationToken);
    public Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken);
    public Task<bool> InsertManyAsync(List<TEntity> objs, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity obj, CancellationToken cancellationToken);
    public Task<int> UpdateRangeAsync(List<TEntity> obj, CancellationToken cancellationToken);
    public Task<bool> DeleteById(Guid entityId, CancellationToken cancellationToken);
    public Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllAsync(bool noTracking, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking, CancellationToken cancellationToken);
    public Task<List<TEntity>> GetAllByIdsAsync(List<Guid> ids, bool noTracking, CancellationToken cancellationToken);

    public Task<List<TEntity>> GetAllWithRelatedDataAsync<TIncluded>
    (
        bool noTracking,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, IEnumerable<TIncluded>>> includeProperty,
        Expression<Func<TIncluded, object>> thenIncludeProperty
    );
    
    public Task<List<TEntity>> GetAllWithRelatedDataAsync(bool noTracking, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties);
    public Task<TEntity?> GetByIdWithRelatedDataAsync(Guid entityId, bool noTracking, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties);
}