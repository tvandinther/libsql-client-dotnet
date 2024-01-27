namespace Libsql.Client.Tests;

public class EmbeddedReplicaTests
{
    [Fact(Skip = "Not implemented")]
    public async Task CanConnectAndQueryReplicaDatabase()
    {
        var db = await DatabaseClient.Create(opts => {
            opts.Url = Environment.GetEnvironmentVariable("LIBSQL_TEST_URL") ?? throw new InvalidOperationException("LIBSQL_TEST_URL is not set");
            opts.AuthToken = Environment.GetEnvironmentVariable("LIBSQL_TEST_AUTH_TOKEN");
            opts.ReplicaPath = "/home/tvandinther/code/libsql-client-dotnet/replica.db";
        });

        await db.Sync();

        var rs = await db.Execute("SELECT COUNT(*) FROM albums");

        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Assert.Equal(347, value.Value);
    }

    [Fact(Skip = "Not implemented")]
    public async Task CanCallSync()
    {
        var db = await DatabaseClient.Create(opts => {
            opts.Url = Environment.GetEnvironmentVariable("LIBSQL_TEST_URL") ?? throw new InvalidOperationException("LIBSQL_TEST_URL is not set");
            opts.AuthToken = Environment.GetEnvironmentVariable("LIBSQL_TEST_AUTH_TOKEN");
            opts.ReplicaPath = "/home/tvandinther/code/libsql-client-dotnet/replica.db";
        });

        await db.Sync();
    }
}
