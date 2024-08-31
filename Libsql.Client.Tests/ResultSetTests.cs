namespace Libsql.Client.Tests;

public class ResultSetTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task Columns_EmptyEnumerable_WhenNonQuery()
    {
        var rs = await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        var columns = rs.Columns;

        Assert.Empty(columns);
    }

    [Fact]
    public async Task Columns_HasLength_WhenNotEmpty()
    {
        var rs = await _db.Query("SELECT 1, 2, 3");
        var columns = rs.Columns;

        Assert.Equal(3, columns.Count());
    }

    [Fact]
    public async Task Columns_NamesAreMarshalled()
    {
        var rs = await _db.Query("SELECT 1 AS [column_one]");
        var columns = rs.Columns;

        Assert.Single(columns);
        Assert.Equal("column_one", columns.First());
    }

    [Fact]
    public async Task Columns_NamesAreMarshalled_WhenSeveral()
    {
        var rs = await _db.Query("SELECT 1 AS [column_one], 2 AS [column_two]");
        var columns = rs.Columns;

        Assert.Equal(2, columns.Count());
        Assert.Equal("column_one", columns.First());
        Assert.Equal("column_two", columns.Last());
    }

    [Fact]
    public async Task LastInsertRowId_Zero_WhenNonQuery()
    {
        var rs = await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");

        Assert.Equal(0, rs.LastInsertRowId);
    }

    [Fact]
    public async Task LastInsertRowId_ReturnsExpectedValue()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        
        var rs = await _db.Query("INSERT INTO `test` DEFAULT VALUES");
        
        Assert.Equal(1, rs.LastInsertRowId);
    }

    [Fact]
    public async Task LastInsertRowId_ReturnsExpectedValue_WhenMultipleInserts()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        
        IResultSet rs;
        for (int i = 0; i < 10; i++)
        {
            rs = await _db.Query("INSERT INTO `test` DEFAULT VALUES");
            Assert.Equal(i + 1, rs.LastInsertRowId);
        }
    }

    [Fact]
    public async Task RowsAffected_Zero_WhenNonQuery()
    {
        var rs = await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");

        Assert.Equal(0ul, rs.RowsAffected);
    }

    [Fact]
    public async Task RowsAffected_ReturnsExpectedValue()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        
        var rs = await _db.Query("INSERT INTO `test` DEFAULT VALUES");
        
        Assert.Equal(1ul, rs.RowsAffected);
    }

    [Fact]
    public async Task RowsAffected_ReturnsExectedValue_WhenMultipleUpdates()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `value` INTEGER)");
        
        for (int i = 0; i < 10; i++)
        {
            var rs = await _db.Execute("INSERT INTO `test` DEFAULT VALUES");
        }

        var rs2 = await _db.Query("UPDATE `test` SET `value` = 1");

        Assert.Equal(10ul, rs2.RowsAffected);
    }
}
