namespace RssFeeder.Server.Infrastructure.Repositories.Contracts;

public interface ISQLiteRepository<TEntity>
{
    public Task InsertAsync(TEntity obj, CancellationToken cancellationToken);
    public Task UpdateAsync(TEntity obj, CancellationToken cancellationToken);
    public void DeleteById(Guid id);
    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);
    public Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken);
}