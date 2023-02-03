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

    public void DeleteById(string id)
    {
        _entitySet.Remove(null);
        _dataContext.SaveChanges();
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
    {
        return _entitySet.ToListAsync(cancellationToken);
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
}