using Libsql.Client;

// Create a database client using the static factory method
using var dbClient = await DatabaseClient.Create(opts => {
    opts.Url = ":memory:";
});


// Execute SQL statements directly
var rs = await dbClient.Query("CREATE TABLE IF NOT EXISTS `users` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `height` REAL, `data` BLOB)");


// Read the results by using the IResultSet interface
var rs1 = await dbClient.Query("INSERT INTO `users` (`name`, `height`, `data`) VALUES ('John Doe', 182.6, X'a4c7b8e21d3f50a6b9d2e8f7c1349a0b5c6d7e218349b6d012c71e8f9a093fed'), ('Jane Doe', 0.5, X'00')");
Console.WriteLine($"Inserted {rs1.RowsAffected} rows");
Console.WriteLine($"Last inserted id: {rs1.LastInsertRowId}");
var rs2 = await dbClient.Query("SELECT `id`, `name`, `height`, `data` FROM `users`");
PrintTable(rs2);


// Using positional arguments
var searchString = "hn";
var rs3 = await dbClient.Query("SELECT `id`, `name`, `height`, `data` FROM `users` WHERE `name` LIKE concat('%', ?, '%')", searchString);
PrintTable(rs3);


// Map rows to User records using type declaration pattern matching
var users = rs2.Rows.Select(ToUser);

User ToUser(IEnumerable<Value> row)
{
    var rowArray = row.ToArray();

    if (
        rowArray[0] is Integer { Value: var id } && 
        rowArray[1] is Text { Value: var name } && 
        rowArray[2] is Real { Value: var height } && 
        rowArray[3] is Blob { Value: var data })
    {
        return new User(id, name, height, data);   
    }

    throw new ArgumentException();
}

void PrintTable(IResultSet rs)
{
    Console.WriteLine();
    Console.WriteLine(string.Join(", ", rs.Columns));
    Console.WriteLine("------------------------");
    Console.WriteLine(string.Join("\n", rs.Rows.Select(row => string.Join(", ", row.Select(x => x.ToString())))));
}

record User(int Id, string Name, double? Height, byte[] Data);
