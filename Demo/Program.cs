using LibsqlClient;

var dbClient = DatabaseClient.Create();

var rs = await dbClient.Execute("CREATE TABLE IF NOT EXISTS `users` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `height` REAL, `data` BLOB)");
Console.WriteLine(rs);
var rs1 = await dbClient.Execute("INSERT INTO `users` (`name`, `height`, `data`) VALUES ('John Doe', 182.6, X'a4c7b8e21d3f50a6b9d2e8f7c1349a0b5c6d7e218349b6d012c71e8f9a093fed'), ('Jane Doe', 0.5, X'00')");
var rs2 = await dbClient.Execute("SELECT `id`, `name`, `height`, `data` FROM `users`");

Console.WriteLine(string.Join(", ", rs2.Columns));
Console.WriteLine("------------------------");
Console.WriteLine(string.Join("\n", rs2.Rows.Select(row => string.Join(", ", row.Select(x => x.ToString())))));

// var user = ToUser(rs2.Rows.First().ToArray());
// Console.WriteLine(user);

User ToUser(Value[] row)
{
    if (
        row[0] is Integer { Value: var id } && 
        row[1] is Text { Value: var name } && 
        row[2] is Real { Value: var height } && 
        row[3] is Blob { Value: var data })
    {
        return new User(id, name, height, data);   
    }

    throw new ArgumentException();
}

record User(int Id, string Name, double? Height, byte[] Data);