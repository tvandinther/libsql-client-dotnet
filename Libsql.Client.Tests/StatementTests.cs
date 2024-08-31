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
        statement.Bind(new Integer(1));
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);

        Assert.Equal<int>(1, integer);
    }

    [Fact]
    public async Task Statement_CanBind_Real()
    {
        var statement = await _db.Prepare("SELECT ?");

        statement.Bind(0.5);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Real>(value);

        Assert.Equal<double>(0.5, integer);
    }

    [Fact]
    public async Task Statement_CanBind_Text()
    {
        var statement = await _db.Prepare("SELECT ?");

        statement.Bind("hello");
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);

        Assert.Equal("hello", text);
    }
}