using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace Libsql.Client.Tests;

public class DatabaseFixture : IDisposable
{
    public DatabaseFixture()
    {
        DatabaseContainer = new ContainerBuilder()
            .WithImage("tvandinther/turso:v0.87.7")
            .WithCommand("dev", "--db-file", "/data/chinook.db")
            .WithResourceMapping("chinook.db", "/data")
            .WithPortBinding(8080, true)
            .Build();
    }

    public IContainer DatabaseContainer { get; }

    public async void Dispose()
    {
        await DatabaseContainer.DisposeAsync();
    }

}

public class RemoteTests : IClassFixture<DatabaseFixture>
{
    public IDatabaseClient DatabaseClient { get; }

    public RemoteTests(DatabaseFixture fixture)
    {
        var databaseContainer = fixture.DatabaseContainer;
        databaseContainer.StartAsync().Wait();
        DatabaseClient = Libsql.Client.DatabaseClient.Create(opts => {
            opts.Url = $"http://{databaseContainer.Hostname}:{databaseContainer.GetMappedPublicPort(8080)}";
            opts.AuthToken = "";
        }).Result;
    }


    [Fact]
    public async Task CanConnectAndQueryRemoteDatabase()
    {
        var rs = await DatabaseClient.Execute("SELECT COUNT(*) FROM tracks");
        
        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Console.WriteLine(value.Value);
        Assert.Equal(3503, value.Value);
    }
}
