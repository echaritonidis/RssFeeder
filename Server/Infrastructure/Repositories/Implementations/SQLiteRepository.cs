using Microsoft.EntityFrameworkCore;
using RssFeeder.Server.Infrastructure.Database;
using RssFeeder.Server.Infrastructure.Model;
using RssFeeder.Server.Infrastructure.Repositories.Contracts;

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

    public void DeleteById(Guid id)
    {
        var obj = _entitySet.First(p => p.Id == id);
        _entitySet.Remove(obj);
        _dataContext.SaveChanges();
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _entitySet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task InsertAsync(TEntity obj, CancellationToken cancellationToken)
    {
        await _entitySet.AddAsync(obj);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity obj, CancellationToken cancellationToken)
    {
        _entitySet.Update(obj);
        await _dataContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _entitySet.AsNoTracking().Where(x => x.Id == id).SingleOrDefaultAsync(cancellationToken);
    }
}