namespace Libsql.Client.Tests;

public class StatementTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    public StatementTests()
    {
        _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        _db.Execute("INSERT INTO `test` VALUES ('a', 'b', 'c')");
    }

    [Fact]
    public async Task Prepare_ReturnedStatement_CanBeExecuted()
    {
        var statement = await _db.Prepare("SELECT `id` FROM `test` WHERE `name` = ?");

        Assert.NotNull(statement);
        Assert.IsAssignableFrom<IStatement>(statement);
    }

    [Fact]
    public async Task Statement_CanBind_Integer()
    {
        var statement = await _db.Prepare("SELECT ?");
        var expected = 1;

        statement.Bind(new Integer(expected));
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);

        Assert.Equal<int>(expected, integer);
    }

    [Fact]
    public async Task Statement_CanBind_Real()
    {
        var statement = await _db.Prepare("SELECT ?");
        var expected = 0.5;

        statement.Bind(expected);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Real>(value);

        Assert.Equal<double>(expected, integer);
    }

    [Fact]
    public async Task Statement_CanBind_Text()
    {
        var statement = await _db.Prepare("SELECT ?");
        var expected = "hello";

        statement.Bind(expected);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);

        Assert.Equal(expected, text);
    }

    [Fact]
    public async Task Statement_CanBind_Blob()
    {
        var statement = await _db.Prepare("SELECT ?");
        var expected = new byte[]{30, 9, 42, 76};

        statement.Bind(expected);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var blob = Assert.IsType<Blob>(value);

        Assert.Equal<byte[]>(expected, blob);
    }

    [Fact]
    public async Task Statement_CanBind_Null()
    {
        var statement = await _db.Prepare("SELECT ?");

        statement.BindNull();
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();

        Assert.IsType<Null>(value);
    }
}