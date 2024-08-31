namespace Libsql.Client.Tests;

public class RowsTests : IDisposable
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task IteratedRows_Throws_WhenCreate()
    {
        async Task<IEnumerable<IEnumerable<Value>>> Action() {
            var rs = await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT)");

            return rs.Rows.ToList();
        }
        
        await Assert.ThrowsAsync<LibsqlException>(Action);
    }

    [Fact]
    public async Task IteratedRows_Throws_WhenCreateIfNotExists()
    {
        var rs = await _db.Query("CREATE TABLE IF NOT EXISTS `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        
        Assert.Empty(rs.Rows);
    }

    [Fact]
    public async Task Rows_Empty_WhenInsert()
    {
        await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");

        var rs = await _db.Query("INSERT INTO `test` (`name`) VALUES ('libsql')");
        
        Assert.Empty(rs.Rows);
    }

    [Fact]
    public async Task Rows_Empty_WhenUpdate()
    {
        await _db.Query("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        var rs = await _db.Query("INSERT INTO `test` (`name`) VALUES ('libsql')");
        Assert.Equal(1ul, rs.RowsAffected);

        var rs2 = await _db.Query("UPDATE `test` SET `name` = 'libsql2' WHERE id = 1");
        
        Assert.Empty(rs2.Rows);
    }
    
    [Fact]
    public async Task Rows_WhenEmpty()
    {
        var rs = await _db.Query("SELECT 1 WHERE 1 = 0");
        var rows = rs.Rows;
        
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Rows_CanIterateTwice()
    {
        var rs = await _db.Query("SELECT 1");
        
        var firstArray = rs.Rows.ToArray();
        var secondArray = rs.Rows.ToArray();
        
        Assert.Equal(firstArray[0], secondArray[0]);
    }

    [Fact]
    public async Task Rows_CanPartiallyIterateTwice()
    {
        var rs = await _db.Query("SELECT 1 UNION SELECT 2 UNION SELECT 3");
        
        var firstArray = rs.Rows.Take(2).ToArray();
        var secondArray = rs.Rows.ToArray();
        
        Assert.Equal(firstArray[0], secondArray[0]);
        Assert.Equal(firstArray[1], secondArray[1]);
        Assert.Equal(3, secondArray.Length);
        var expected = new []{new Integer(1), new Integer(2), new Integer(3)};
        Assert.Equal(expected, secondArray.Select(x => x.First() as Integer).ToArray());
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}