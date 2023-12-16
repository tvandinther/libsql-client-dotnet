namespace Libsql.Client.Tests;

public class RowsTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;
    
    [Fact]
    public async Task Rows_WhenEmpty()
    {
        var rs = await _db.Execute("SELECT 1 WHERE 1 = 0");
        var rows = rs.Rows;
        
        Assert.Empty(rows);
    }

    [Fact]
    public async Task Rows_CanIterateTwice()
    {
        var rs = await _db.Execute("SELECT 1");
        
        var firstArray = rs.Rows.ToArray();
        var secondArray = rs.Rows.ToArray();
        
        Assert.Equal(firstArray[0], secondArray[0]);
    }

    [Fact]
    public async Task Rows_CanPartiallyIterateTwice()
    {
        var rs = await _db.Execute("SELECT 1 UNION SELECT 2 UNION SELECT 3");
        
        var firstArray = rs.Rows.Take(2).ToArray();
        var secondArray = rs.Rows.ToArray();
        
        Assert.Equal(firstArray[0], secondArray[0]);
        Assert.Equal(firstArray[1], secondArray[1]);
        Assert.Equal(3, secondArray.Length);
        var expected = new []{new Integer(1), new Integer(2), new Integer(3)};
        Assert.Equal(expected, secondArray.Select(x => x.First() as Integer).ToArray());
    }
}