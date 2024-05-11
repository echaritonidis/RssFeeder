using Marten;
using Marten.Schema;
using RssFeeder.Server.Infrastructure.Model;

namespace RssFeeder.Server.Infrastructure.Database;

public class InitialData: IInitialData
{
    private readonly object[] _initialData;

    public InitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await using var session = store.LightweightSession();
        // Marten UPSERT will cater for existing records
        session.Store(_initialData);
        await session.SaveChangesAsync();
    }
}

public static class InitialDatasets
{
    public static readonly FeedGroup[] FeedGroups =
    {
        new FeedGroup
        {
            Id = Guid.Parse("2303d87c-a388-4250-807c-7b5279623b0e"), 
            Initial = true,
            Color = "#000", 
            Title = "Unclassified", 
            Description = "Generic specific category or topic"
        }
    };
}