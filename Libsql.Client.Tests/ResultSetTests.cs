namespace Libsql.Client.Tests;

public class ResultSetTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create();

    [Fact]
    public async Task Columns_EmptyEnumerable_WhenNonQuery()
    {
        var rs = await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        var columns = rs.Columns;

        Assert.Empty(columns);
    }

    [Fact]
    public async Task Columns_HasLength_WhenNotEmpty()
    {
        var rs = await _db.Execute("SELECT 1, 2, 3");
        var columns = rs.Columns;

        Assert.Equal(3, columns.Count());
    }

    [Fact]
    public async Task Columns_NamesAreMarshalled()
    {
        var rs = await _db.Execute("SELECT 1 AS [column_one]");
        var columns = rs.Columns;

        Assert.Single(columns);
        Assert.Equal("column_one", columns.First());
    }

    [Fact]
    public async Task Columns_NamesAreMarshalled_WhenSeveral()
    {
        var rs = await _db.Execute("SELECT 1 AS [column_one], 2 AS [column_two]");
        var columns = rs.Columns;

        Assert.Equal(2, columns.Count());
        Assert.Equal("column_one", columns.First());
        Assert.Equal("column_two", columns.Last());
    }
}
