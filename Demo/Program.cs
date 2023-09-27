using LibsqlClient;

Console.WriteLine(Client.Add(1, 1));

var dbClient = DatabaseClient.Create(opts =>
{
    opts.Url = "http://localhost:8080";
    opts.AuthToken = "secret";
    opts.UseHttps = true;
});

var rs = await dbClient.Execute("SELECT `id`, `name` FROM `users`");
var users = rs.Rows.Select(row => new User(
    Id: (int)row[0],
    Name: (string)row[1])
);

Console.WriteLine(string.Join(", ", users.Select(u => $"{u.Name} ({u.Id})")));
record User(int Id, string Name);