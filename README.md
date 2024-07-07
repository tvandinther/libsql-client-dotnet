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

### Planned Features

- Positional and named arguments.
- Embedded replicas.
- Prepared statements.
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
await dbClient.Execute("SELECT name FROM users WHERE id = ?", userId);
```

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

var result = await dbClient.Execute("SELECT * FROM users");

var users = result.Rows.Select(ToUser);
```

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
