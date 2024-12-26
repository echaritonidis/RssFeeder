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

    public Task<TEntity?> GetOnePredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return this._querySession.Query<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return this._querySession
            .Query<TEntity>()
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return this._querySession.Query<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<TEntity>> GetAllByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        return this._querySession.Query<TEntity>().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
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
}