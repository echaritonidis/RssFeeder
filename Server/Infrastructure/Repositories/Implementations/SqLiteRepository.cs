using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations;

public class SqLiteRepository<TEntity> : ISqLiteRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DataContext _dataContext;
    private readonly DbSet<TEntity> _entitySet;

    public SqLiteRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
        _entitySet = dataContext.Set<TEntity>();
    }
    
    public async Task Commit(CancellationToken cancellationToken) => await _dataContext.SaveChangesAsync(cancellationToken);

    private IQueryable<TEntity> GetQueryable(bool noTracking)
    {
        return noTracking ? _entitySet.AsNoTracking() : _entitySet;
    }
    
    public Task<List<TEntity>> GetAllAsync(bool noTracking, CancellationToken cancellationToken)
    {
        return this.GetQueryable(noTracking).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, bool noTracking, CancellationToken cancellationToken)
    {
        return this.GetQueryable(noTracking).Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetAllByIdsAsync(List<Guid> ids, bool noTracking, CancellationToken cancellationToken)
    {
        return this.GetQueryable(noTracking).Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _dataContext.Entry(obj).State = EntityState.Added;
        await _dataContext.SaveChangesAsync(cancellationToken);

        return obj.Id;
    }

    public async Task<bool> InsertManyAsync(List<TEntity> objs, CancellationToken cancellationToken)
    {
        objs.ForEach(obj => _dataContext.Entry(obj).State = EntityState.Added);

        await _dataContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task UpdateAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _dataContext.Entry(obj).State = EntityState.Modified;
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateRangeAsync(List<TEntity> obj, CancellationToken cancellationToken)
    {
        _entitySet.UpdateRange(obj);
        return await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteById(Guid entityId, CancellationToken cancellationToken)
    {
        var entityToDelete = await _entitySet.FindAsync(new object[] { entityId }, cancellationToken);
        
        if (entityToDelete == null) return false;
        
        _entitySet.Remove(entityToDelete);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken)
    {
        var entitiesToDelete = await _entitySet.Where(x => entityIds.Contains(x.Id)).ToListAsync(cancellationToken);
        
        if (!(entitiesToDelete?.Count > 0)) return false;
        
        _entitySet.RemoveRange(entitiesToDelete);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<List<TEntity>> GetAllWithRelatedDataAsync<TIncluded>
    (
        bool noTracking,
        CancellationToken cancellationToken, 
        Expression<Func<TEntity, IEnumerable<TIncluded>>> includeProperty,
        Expression<Func<TIncluded, object>> thenIncludeProperty
    )
    {
        var query = this.GetQueryable(noTracking);

        return await query
            .Include(includeProperty)
            .ThenInclude(thenIncludeProperty)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TEntity>> GetAllWithRelatedDataAsync(bool noTracking, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = this.GetQueryable(noTracking);

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdWithRelatedDataAsync(Guid entityId, bool noTracking, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[]? includeProperties)
    {
        var query = this.GetQueryable(noTracking).Where(x => x.Id == entityId);

        if (includeProperties != null)
        {
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}