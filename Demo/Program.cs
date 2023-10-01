using LibsqlClient;

var dbClient = DatabaseClient.Create();

// var dbClient2 = DatabaseClient.Create(opts =>
// {
//     opts.Url = ":memory:";
// });

var rs = await dbClient.Execute("CREATE TABLE `users` (`id` INTEGER PRIMARY KEY AUTOINCREMENT, `name` TEXT NOT NULL)");
Console.WriteLine(rs.RowsAffected);

var rs1 = await dbClient.Execute("INSERT INTO `users` (`name`) VALUES ('John Doe'), ('Jane Doe')");
Console.WriteLine(rs1.RowsAffected);
Console.WriteLine(rs1.LastInsertRowId);
var rs2 = await dbClient.Execute("SELECT `id`, `name` FROM `users`");
Console.WriteLine(string.Join(", ", rs2.Columns));
Console.WriteLine("------------------------");
Console.WriteLine(string.Join("\n", rs2.Rows.Select(row => string.Join(", ", row))));
// var dbClient = DatabaseClient.Create(opts =>
// {
//     opts.Url = "http://localhost:8080";
//     opts.AuthToken = "secret";
//     opts.UseHttps = true;
// });
//
// var rs = await dbClient.Execute("SELECT `id`, `name` FROM `users`");
// var users = rs.Rows.Select(row => new User(
//     Id: (int)row[0],
//     Name: (string)row[1])
// );
//
// Console.WriteLine(string.Join(", ", users.Select(u => $"{u.Name} ({u.Id})")));
// record User(int Id, string Name);