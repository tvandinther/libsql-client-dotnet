namespace Libsql.Client.Tests;

public class RemoteTests : IClassFixture<DatabaseContainer>
{
    public IDatabaseClient DatabaseClient { get; }

    public RemoteTests(DatabaseContainer fixture)
    {
        Skip.IfNot(OperatingSystem.IsLinux(), "Remote tests run only on Linux.");

        var databaseContainer = fixture.Container;
        databaseContainer.StartAsync().Wait();
        DatabaseClient = Libsql.Client.DatabaseClient.Create(opts => {
            opts.Url = $"http://{databaseContainer.Hostname}:{databaseContainer.GetMappedPublicPort(8080)}";
            opts.AuthToken = "";
        }).Result;
    }


    [SkippableFact]
    public async Task CanConnectAndQueryRemoteDatabase()
    {
        var rs = await DatabaseClient.Execute("SELECT COUNT(*) FROM tracks");
        
        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Console.WriteLine(value.Value);
        Assert.Equal(3503, value.Value);
    }
}
