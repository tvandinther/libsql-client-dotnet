namespace Libsql.Client.Tests;

public class StatementTests
{
    private readonly IDatabaseClient _db = DatabaseClient.Create().Result;

    [Fact]
    public async Task Statement_CanBind_Integer()
    {
        using var statement = await _db.Prepare("SELECT ?");
        var expected = 1;

        statement.Bind(new Integer(expected), 1);
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

        statement.Bind(expected, 1);
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

        statement.Bind(expected, 1);
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

        statement.Bind(expected, 1);
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

        statement.BindNull(1);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();

        Assert.IsType<Null>(value);
    }

    [Fact]
    public async Task Statement_CanBind_NNNIndex()
    {
        using var statement = await _db.Prepare("SELECT ?123");
        var expected = "Parameter annotated as the 123rd position";

        statement.Bind(expected, 123);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();

        var text = Assert.IsType<Text>(value);
        Assert.Equal(expected, text.Value);
    }

    [Theory]
    [InlineData("SELECT 1", 0)]
    [InlineData("SELECT ?", 1)]
    [InlineData("SELECT ?, ?", 2)]
    [InlineData("SELECT ?, ?, ?", 3)]
    [InlineData("SELECT ?10, ?11, ?33", 33)]
    [InlineData("SELECT ?10, ?33, ?11", 33)]
    [InlineData("SELECT ?, ?, ?2", 2)]
    [InlineData("SELECT ?, ?, ?2, ?, ?", 4)]
    public async Task Statement_CanGetParameterCount(string query, int expected)
    {
        using var statement = await _db.Prepare(query);

        var parameterCount = statement.ParameterCount;

        Assert.Equal(expected, parameterCount);
    }

    [Fact]
    public async Task Statement_BoundValuesCount_IsCorrect()
    {
        using var statement = await _db.Prepare("SELECT ?, ?, ?");
        statement.Bind("One", 1);
        statement.Bind("Two", 2);
        statement.Bind("Three", 3);

        var numberOfBoundValues = statement.BoundValuesCount;

        Assert.Equal(3, numberOfBoundValues);
    }

    [Fact]
    public async Task Database_CanQuery_Statements()
    {
        using var statement = await _db.Prepare("SELECT ?");
        var expected = 1;

        statement.Bind(new Integer(expected), 1);
        var rs = await _db.Query(statement);
        var row = rs.Rows.First();
        var value = row.First();
        var integer = Assert.IsType<Integer>(value);

        Assert.Equal<int>(expected, integer);
    }

    [Theory]
    // [InlineData("SELECT ?123", "123")] // TODO: throws exception, make test for this
    // [InlineData("SELECT ?foo", "123")] // TODO: throws exception, make test for this
    [InlineData("SELECT :test", "test")]
    [InlineData("SELECT @test", "test")]
    [InlineData("SELECT $test", "test")]
    public async Task Statement_canGet_ParameterName(string query, string expected)
    {
        using var statement = await _db.Prepare(query);

        var name = statement.GetParameterNameAtIndex(1);

        Assert.Equal(expected, name);
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

    [Fact]
    public async Task Statement_CanBeReset()
    {
        await _db.Execute("CREATE TABLE `test` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT)");
        await _db.Execute("INSERT INTO `test` (`name`) VALUES ('a'), ('b'), ('c')");

        using var statement = await _db.Prepare("SELECT `name` FROM `test` WHERE `id` = ?");
        var firstExpected = "a";
        var secondExpected = "b";

        statement.Bind(new Integer(1), 1);
        var rs = await statement.Query();
        var row = rs.Rows.First();
        var value = row.First();
        var text = Assert.IsType<Text>(value);

        Assert.Equal(firstExpected, text);

        statement.Reset();

        statement.Bind(new Integer(2), 1);
        var rs2 = await statement.Query();
        var row2 = rs2.Rows.First();
        var value2 = row2.First();
        var text2 = Assert.IsType<Text>(value2);

        Assert.Equal(secondExpected, text2);
    }
}