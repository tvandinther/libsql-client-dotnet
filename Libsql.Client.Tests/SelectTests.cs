namespace Libsql.Client.Tests;

public class SelectTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create();

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
    public async Task SelectIntType_WhenNull()
    {
        await _db.Execute("CREATE TABLE IF NOT EXISTS test (id INTEGER, value INTEGER)");
        await _db.Execute("INSERT INTO test (value) VALUES (NULL)");
        
        var rs = await _db.Execute("SELECT value FROM test");
        var row = rs.Rows.First();
        var value = row.First();
        
        Assert.IsType<Null>(value);
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

    [Fact]
    public async Task SelectTextType_Unicode()
    {
        var rs = await _db.Execute("SELECT 'Привет, мир!'");
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);
        
        Assert.Equal("Привет, мир!", text.Value);
    }

    [Fact]
    public async Task SelectTextType_WhenNull()
    {
        await _db.Execute("CREATE TABLE IF NOT EXISTS test (id INTEGER, value TEXT)");
        await _db.Execute("INSERT INTO test (value) VALUES (NULL)");
        
        var rs = await _db.Execute("SELECT value FROM test");
        var row = rs.Rows.First();
        var value = row.First();
        
        Assert.IsType<Null>(value);
    }
    
    [Fact]
    public async Task SelectRealType()
    {
        var rs = await _db.Execute("SELECT 0.177");
        var row = rs.Rows.First();
        var value = row.First();
        var real = Assert.IsType<Real>(value);
        Assert.Equal(0.177d, real.Value);
    }
    
    [Fact]
    public async Task SelectRealType_WhenNull()
    {
        await _db.Execute("CREATE TABLE IF NOT EXISTS test (id INTEGER, value REAL)");
        await _db.Execute("INSERT INTO test (value) VALUES (NULL)");
        
        var rs = await _db.Execute("SELECT value FROM test");
        var row = rs.Rows.First();
        var value = row.First();
        
        Assert.IsType<Null>(value);
    }
    
    [Fact]
    public async Task SelectBlobType()
    {
        var expected = new byte[]{30, 9, 42, 76};
        var rs = await _db.Execute("SELECT X'1e092a4c'");
        var row = rs.Rows.First();
        var value = row.First();
        var blob = Assert.IsType<Blob>(value);
        Assert.Equal(expected, blob.Value);
    }
    
    [Fact]
    public async Task SelectBlobType_WhenNull()
    {
        await _db.Execute("CREATE TABLE IF NOT EXISTS test (id INTEGER, value BLOB)");
        await _db.Execute("INSERT INTO test (value) VALUES (NULL)");
        
        var rs = await _db.Execute("SELECT value FROM test");
        var row = rs.Rows.First();
        var value = row.First();
        
        Assert.IsType<Null>(value);
    }
    
    [Fact]
    public async Task SelectNullType()
    {
        var rs = await _db.Execute("SELECT NULL");
        var row = rs.Rows.First();
        var value = row.First();
        Assert.IsType<Null>(value);
    }
}
