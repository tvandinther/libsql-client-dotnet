using DotNet.Testcontainers.Containers;

namespace Libsql.Client.Tests;

public class EmbeddedReplicaTests : IClassFixture<DatabaseContainer>
{
    public IDatabaseClient DatabaseClient { get; }

    public EmbeddedReplicaTests(DatabaseContainer fixture)
    {
        Skip.If(fixture.Container is null, "Remote tests run only on Linux.");
        IContainer databaseContainer = fixture.Container!;

        databaseContainer.StartAsync().Wait();
        DatabaseClient = Libsql.Client.DatabaseClient.Create(opts => {
            opts.Url = $"http://{databaseContainer.Hostname}:{databaseContainer.GetMappedPublicPort(8080)}";
            opts.AuthToken = "";
            opts.ReplicaPath = "replica.db";
        }).Result;
    }

    [Fact(Skip = "Not implemented")]
    // [SkippableFact]
    public async Task CanConnectAndQueryReplicaDatabase()
    {
        await DatabaseClient.Sync();

        var rs = await DatabaseClient.Execute("SELECT COUNT(*) FROM albums");

        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Assert.Equal(347, value.Value);
    }

    [Fact(Skip = "Not implemented")]
    // [SkippableFact]
    public async Task CanCallSync()
    {
        await DatabaseClient.Sync();
    }
}
