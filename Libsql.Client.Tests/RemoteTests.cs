namespace Libsql.Client.Tests;

public class RemoteTests
{
    [Fact]
    public async Task CanConnectAndQueryRemoteDatabase()
    {
        var db = await DatabaseClient.Create(opts => {
            opts.Url = Environment.GetEnvironmentVariable("LIBSQL_TEST_URL") ?? throw new InvalidOperationException("LIBSQL_TEST_URL is not set");
            opts.AuthToken = Environment.GetEnvironmentVariable("LIBSQL_TEST_AUTH_TOKEN");
        });

        var rs = await db.Execute("SELECT COUNT(*) FROM tracks");
        
        var count = rs.Rows.First().First();
        var value = Assert.IsType<Integer>(count);
        Console.WriteLine(value.Value);
        Assert.Equal(3503, value.Value);
    }
}
