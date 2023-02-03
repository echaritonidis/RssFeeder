namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ISQLiteRepository<TEntity>
{
    public Task InsertAsync(TEntity obj, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity obj, CancellationToken cancellationToken);
    public void DeleteById(string id);
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);
}