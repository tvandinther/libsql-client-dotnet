namespace LibsqlClient.Test;

public class SelectTests
{
    private readonly IDatabaseClient _db;

    public SelectTests()
    {
        _db = DatabaseClient.Create();
    }
    
    [Fact]
    public async Task SelectIntType()
    {
        var rs = await _db.Execute("SELECT 1");
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);
        Assert.Equal(1, integer.Value);
    }

    [Fact]
    public async Task SelectTextType()
    {
        var rs = await _db.Execute("SELECT 'Hello, World!'");
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);
        Assert.Equal("Hello, World!", text.Value);
    }
}