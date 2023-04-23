using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;
using System.Linq.Expressions;

namespace RssFeeder.Server.Infrastructure.Repositories.Implementations;

public class SQLiteRepository<TEntity> : ISQLiteRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DataContext _dataContext;
    private readonly DbSet<TEntity> _entitySet;

    public SQLiteRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
        _entitySet = dataContext.Set<TEntity>();
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _entitySet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetAllPredicatedAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return _entitySet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public Task<List<TEntity>> GetAllByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        return _entitySet.AsNoTracking().Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);
    }

    public async Task<Guid> InsertAsync(TEntity obj, CancellationToken cancellationToken)
    {
        await _entitySet.AddAsync(obj);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return obj.Id;
    }

    public async Task UpdateAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _entitySet.Update(obj);
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

        if (entityToDelete != null)
        {
            _entitySet.Remove(entityToDelete);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task<bool> DeleteByIds(List<Guid> entityIds, CancellationToken cancellationToken)
    {
        var entitiesToDelete = await _entitySet.Where(x => entityIds.Contains(x.Id)).ToListAsync(cancellationToken);

        if (entitiesToDelete?.Count > 0)
        {
            _entitySet.RemoveRange(entitiesToDelete);
            await _dataContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task<List<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _entitySet.AsNoTracking();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> GetByIdWithRelatedDataAsync(Guid entityId, CancellationToken cancellationToken, params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = _entitySet.AsNoTracking().Where(x => x.Id == entityId);

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}