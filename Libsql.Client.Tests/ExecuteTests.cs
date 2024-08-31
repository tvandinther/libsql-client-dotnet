namespace Libsql.Client.Tests;

public class ExecuteTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task CreateTable_NoRowsAffected()
    {
        var rowsAffected = await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");

        Assert.Equal(0ul, rowsAffected);
    }

    

    [Fact]
    public async Task RowsAffected_ReturnsOne_WhenRepeatedInserts()
    {
        await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        
        for (int i = 0; i < 10; i++)
        {
            var rowsAffected = await _db.Execute("INSERT INTO `test` DEFAULT VALUES");
            Assert.Equal(1ul, rowsAffected);
        }
    }

    [Fact]
    public async Task RowsAffected_ReturnsExpectedValue()
    {
        await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");
        
        var rs = await _db.Query("INSERT INTO `test` DEFAULT VALUES");
        
        Assert.Equal(1ul, rs.RowsAffected);
    }

    [Fact]
    public async Task RowsAffected_ReturnsExectedValue_WhenMultipleUpdates()
    {
        await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `value` INTEGER)");
        
        for (int i = 0; i < 10; i++)
        {
            var rs = await _db.Execute("INSERT INTO `test` DEFAULT VALUES");
        }

        var rs2 = await _db.Query("UPDATE `test` SET `value` = 1");

        Assert.Equal(10ul, rs2.RowsAffected);
    }
}
