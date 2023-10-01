using LibsqlClient;

var dbClient = DatabaseClient.Create();

var rs = await dbClient.Execute("CREATE TABLE `users` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL, `height` REAL, `data` BLOB)");
var rs1 = await dbClient.Execute("INSERT INTO `users` (`name`, `height`, `data`) VALUES ('John Doe', 182.6, X'0102030405060708090a0b0c0d0e0f'), ('Jane Doe', NULL, NULL)");
var rs2 = await dbClient.Execute("SELECT `id`, `name`, `height`, `data` FROM `users`");

Console.WriteLine(string.Join(", ", rs2.Columns));
Console.WriteLine("------------------------");
Console.WriteLine(string.Join("\n", rs2.Rows.Select(row => string.Join(", ", row.Select(x =>
{
    return x switch
    {
        null => "NULL",
        byte[] bytes => string.Join("", bytes.Select(b => b.ToString("x2"))),
        _ => x.ToString()
    };
})))));