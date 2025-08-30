namespace Libsql.Client.Tests;

public class DatabaseClientTests
{
    [Theory]
    [InlineData("file:testdb.sqlite")]
    [InlineData(":memory:")]
    public async Task Create_WithValidPaths_ShouldCreateDatabaseClient(string path)
    {
        var client = await DatabaseClient.Create(path);

        Assert.NotNull(client);
        Assert.IsAssignableFrom<IDatabaseClient>(client);
    }

    [Fact]
    public async Task Create_WithLocalPath_ShouldCreateDatabaseClient()
    {
        var filePath = Path.GetFullPath("testdb.sqlite");

        var client = await DatabaseClient.Create(filePath);

        Assert.NotNull(client);
        Assert.IsAssignableFrom<IDatabaseClient>(client);
    }

    [Fact]
    public async Task Create_WithNullPath_ShouldThrowArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => DatabaseClient.Create((string)null!));
    }
}