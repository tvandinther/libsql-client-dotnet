![tests](https://github.com/tvandinther/libsql-client-dotnet/actions/workflows/test.yaml/badge.svg)

# Libsql.Client

A .NET client library for libsql.

**This project is still in early development and not ready for production use.**

### Currently Supported Features

- Creating a database:
  - In memory.
  - From file.
  - From connection string.
- Executing SQL statements:
  - Non-parameterised.
  - With positional parameters
- Prepared statements.

### Planned Features

- Named arguments.
- Embedded replicas.
- Batched statements.
- Transactions.

## Usage

For an example, see the Demo project in the repository.

### Creating a Database

```csharp
// Create an in-memory database.
var dbClient = DatabaseClient.Create(opts => {
    opts.Url = ":memory:";
});
```

### Executing SQL Statements

Using direct queries
```csharp
await dbClient.Execute("CREATE TABLE users (id INTEGER PRIMARY KEY, name TEXT, height REAL)");
```

Using positional arguments
```csharp
await dbClient.Query("SELECT name FROM users WHERE id = ?", userId);
```

Using prepared statements
```csharp
var statement = dbClient.Prepare("SELECT name FROM users WHERE id = ?");
await statement.Query(userId);
```

#### `Execute` vs. `Query`

`Execute` returns only the number of affected rows and is intended for statements where further detail is not necessary. A `LibSqlException` will be thrown if you use `Execute` on a statement that returns rows.

`Query` returns a more useful `IResultSet` object which can be read for additional information such as the number of affected rows, the last inserted row ID, the column names, and the rows themselves.

### Querying the Database

```csharp
User ToUser(IEnumerable<Value> row)
{
    var rowArray = row.ToArray();

    if (
        rowArray[0] is Integer { Value: var id } && 
        rowArray[1] is Text { Value: var name } && 
        rowArray[2] is Real { Value: var height }
    {
        return new User(id, name, height);   
    }

    throw new ArgumentException();
}

var result = await dbClient.Query("SELECT * FROM users");

var users = result.Rows.Select(ToUser);
```

### Prepared Statements

The following creates a prepared statement.
```csharp
var statement = dbClient.Prepare("SELECT * FROM users where userId = ?");
```
You can then bind positional arguments in order.
```csharp
statement.Bind(new Integer(1))
```
Statement execution can be done in two ways, both are the same.
```csharp
IResultSet result;
result = statement.Query();
result = dbClient.Query(statement);
```
You can also query the number of values already bound to the statement.
```csharp
statement.Bind(0.5);
statement.Bind("libsql");
var numberOfBoundValues = statement.BoundValuesCount;
Console.WriteLine(numberOfBoundValues) // 2
```
> Prepared statements are held resources. `IStatement` implements the `IDisposable` interface. Make sure you manage its lifetime correctly.

### Closing the Database

```csharp
dbClient.Dispose();
```

or with a `using` statement:

```csharp
using (var dbClient = DatabaseClient.Create(opts => {
    opts.Url = ":memory:";
}))
{
    // ...
}
```

## Disclaimer

This project is still in early development and not ready for production use. The API is subject to include breaking changes on minor versions until version 1.0.

The full test suite is run only on a Linux x64 platform. Most of the test suite is run on Linux, Windows, and macOS x64 platforms. The test suite runs on .NET 7.

## Progress
- A database can be created:
  - [x] In memory.
  - [x] From file.
  - [x] From connection string.
- [x] A database can be destroyed/closed/deallocated.
- [ ] An embedded replica can be created.
  - [ ] An embeded replica can be synced.
- The database can execute SQL statements:
  - [x] Non-parameterised.
  - [x] Parameterised with positional arguments.
  - [ ] Parameterised with named arguments.
- [ ] Prepared statements.
- [ ] Batched statements.
- [ ] Transactions.
- [x] A result set is returned from an execution.
  - [x] With the column names.
  - [x] With an enumerable of enumerable (rows) of typed boxed values.
  - [x] With the number of affected rows.
  - [x] With the last inserted row id.
