using DotNet.Testcontainers.Containers;

namespace Libsql.Client.Tests;

public class RemoteTests : IClassFixture<DatabaseContainer>, IDisposable
{
    public IDatabaseClient DatabaseClient { get; }

    public RemoteTests(DatabaseContainer fixture)
    {
        Skip.If(fixture.Container is null, "Remote tests run only on Linux.");
        IContainer databaseContainer = fixture.Container!;

        databaseContainer.StartAsync().Wait();
        DatabaseClient = Libsql.Client.DatabaseClient.Create(opts => {
            opts.Url = $"http://{databaseContainer.Hostname}:{databaseContainer.GetMappedPublicPort(8080)}";
            opts.AuthToken = "";
        }).Result;
    }


    [SkippableFact]
    public async Task CanConnectAndQueryRemoteDatabase()
    {
        var rs = await DatabaseClient.Query("SELECT COUNT(*) FROM tracks");
        
        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Assert.Equal(3503, value.Value);
    }

    public void Dispose()
    {
        DatabaseClient.Dispose();
        GC.SuppressFinalize(this);
    }
}
