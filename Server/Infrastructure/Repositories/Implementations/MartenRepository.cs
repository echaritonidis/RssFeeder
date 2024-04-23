using Marten;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations;

public class MartenRepository<TEntity> : IMartenRepository<TEntity> where TEntity : BaseEntity
{
    private readonly IQuerySession _querySession;
    private readonly IDocumentSession _documentSession;

    public MartenRepository(IQuerySession querySession, IDocumentSession documentSession)
    {
        _querySession = querySession;
        _documentSession = documentSession;
    }
    
    public async Task Commit(CancellationToken cancellationToken) => await _documentSession.SaveChangesAsync(cancellationToken);

    public Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return this._documentSession.LoadManyAsync<TEntity>(new List<string>());
    }

    public Task<IReadOnlyList<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return this._documentSession.Query<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> GetAllByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        return this._documentSession.Query<TEntity>().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _documentSession.Store<TEntity>(obj);
        await _documentSession.SaveChangesAsync(cancellationToken);
        
        return obj.Id;
    }

    public async Task<bool> InsertManyAsync(List<TEntity> objs, CancellationToken cancellationToken)
    {
        objs.ForEach(obj => _documentSession.Store<TEntity>(obj));

        await _documentSession.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task UpdateAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _documentSession.Update<TEntity>(obj);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(List<TEntity> obj, CancellationToken cancellationToken)
    {
        _documentSession.Update(obj);
        await _documentSession.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> DeleteById(Guid entityId, CancellationToken cancellationToken)
    {
        _documentSession.HardDeleteWhere<TEntity>(x => x.Id == entityId);

        return Task.FromResult(true);
    }

    public Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken)
    {
        _documentSession.HardDeleteWhere<TEntity>(x => entityIds.Contains(x.Id));

        return Task.FromResult(true);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllWithRelatedDataAsync<TIncluded>(CancellationToken cancellationToken, Expression<Func<TEntity, object>> includeProperty)
    {
        return await _documentSession
            .Query<TEntity>()
            .Include(includeProperty, (TIncluded item) => {  })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = _documentSession.Query<TEntity>();

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty, (object item) => {}));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdWithRelatedDataAsync(Guid entityId, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = _documentSession.Query<TEntity>().Where(x => x.Id == entityId);

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty, (object item) => {}));
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> GetAllWithRelatedDataAsync<TIncluded>(CancellationToken cancellationToken, Expression<Func<TEntity, IEnumerable<TIncluded>>> includeProperty)
    {
        throw new NotImplementedException();
    }
}