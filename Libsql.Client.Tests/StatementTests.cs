namespace Libsql.Client.Tests;

public class StatementTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task Statement_CanBind_Integer()
    {
        using var statement = await _db.Prepare("SELECT ?");
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
        using var statement = await _db.Prepare("SELECT ?");
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
        using var statement = await _db.Prepare("SELECT ?");
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
        using var statement = await _db.Prepare("SELECT ?");
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
        using var statement = await _db.Prepare("SELECT ?");

        statement.BindNull();
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();

        Assert.IsType<Null>(value);
    }

    [Fact]
    public async Task Statement_BoundValuesCount_IsCorrect()
    {
        using var statement = await _db.Prepare("SELECT ?, ?, ?");
        statement.Bind("One");
        statement.Bind("Two");
        statement.Bind("Three");

        var numberOfBoundValues = statement.BoundValuesCount;

        Assert.Equal(3, numberOfBoundValues);
    }

    [Fact]
    public async Task Database_CanQuery_Statements()
    {
        using var statement = await _db.Prepare("SELECT ?");
        var expected = 1;

        statement.Bind(new Integer(expected));
        var rs = await _db.Query(statement);
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);

        Assert.Equal<int>(expected, integer);
    }

    [Fact]
    public async Task Statement_CanExecute()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        using var statement = await _db.Prepare("INSERT INTO `test` (`name`) VALUES ('a'), ('b'), ('c')");

        var rowsAffected = await statement.Execute();

        Assert.Equal(3ul, rowsAffected);
    }
    

    [Fact]
    public async Task Database_CanExecute_Statements()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        using var statement = await _db.Prepare("INSERT INTO `test` (`name`) VALUES ('a'), ('b'), ('c')");

        var rowsAffected = await _db.Execute(statement);

        Assert.Equal(3ul, rowsAffected);
    }
}