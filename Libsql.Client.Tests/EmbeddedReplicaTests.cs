namespace Libsql.Client.Tests;

public class EmbeddedReplicaTests
{
    [Fact]
    public async Task CanConnectAndQueryRemoteDatabase()
    {
        var db = await DatabaseClient.Create(opts => {
            opts.Url = Environment.GetEnvironmentVariable("LIBSQL_TEST_URL") ?? throw new InvalidOperationException("LIBSQL_TEST_URL is not set");
            opts.AuthToken = Environment.GetEnvironmentVariable("LIBSQL_TEST_AUTH_TOKEN") ?? throw new InvalidOperationException("LIBSQL_TEST_AUTH_TOKEN is not set");
            opts.ReplicaPath = "replica.db";
        });

        var rs = await db.Execute("SELECT COUNT(*) FROM Orders");

        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Assert.Equal(830, value.Value);
    }

    [Fact]
    public async Task CanConnectAndQueryReplicaDatabase()
    {
        var db = await DatabaseClient.Create("replica.db");

        var rs = await db.Execute("SELECT COUNT(*) FROM Orders");

        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Assert.Equal(830, value.Value);
    }
}
