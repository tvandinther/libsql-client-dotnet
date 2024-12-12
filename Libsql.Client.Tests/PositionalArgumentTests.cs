namespace Libsql.Client.Tests;

public class PositionalArgumentTests : IDisposable
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task SingleParameter()
    {
        var rs = await _db.Execute("SELECT ?", 1);
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);
        
        Assert.Equal(1, integer.Value);
    }

    [Fact]
    public async Task MultipleParameters()
    {
        var rs = await _db.Execute("SELECT ?, ?, ?", 1.0, "2", 3);
        var row = rs.Rows.First();
        var integer = Assert.IsType<Integer>(row.Skip(2).First());
        
        Assert.Equal(3, integer.Value);
    }

    [Fact]
    public async Task BindIntParameter()
    {
        var rs = await _db.Execute("SELECT ?", 1);
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);
        
        Assert.Equal(1, integer.Value);
    }

    [Fact]
    public async Task BindRealParameter()
    {
        var rs = await _db.Execute("SELECT ?", 1.0);
        var row = rs.Rows.First();
        var value = row.First();
        var real = Assert.IsType<Real>(value);
        
        Assert.Equal(1.0, real.Value);
    }

    [Fact]
    public async Task BindStringParameter()
    {
        var rs = await _db.Execute("SELECT ?", "hello");
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);
        
        Assert.Equal("hello", text.Value);
    }

    [Fact]
    public async Task BindSingleNullParameter()
    {
        var rs = await _db.Execute("SELECT ?", null);
        var row = rs.Rows.First();
        var value = row.First();
        Assert.IsType<Null>(value);
    }

    [Fact]
    public async Task BindMultipleParametersWithANull()
    {
        var rs = await _db.Execute("SELECT ?, ?, ?", 1, null, 3);
        var row = rs.Rows.First();
        var value = row.Skip(1).First();
        Assert.IsType<Null>(value);
    }

    [Fact]
    public async Task BindBlobParameter()
    {
        var rs = await _db.Execute("SELECT ?", new byte[] { 1, 2, 3 });
        var row = rs.Rows.First();
        var value = row.First();
        var blob = Assert.IsType<Blob>(value);
        
        Assert.Equal(new byte[] { 1, 2, 3 }, blob.Value);
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
}
