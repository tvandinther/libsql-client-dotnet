namespace LibsqlClient;

public static class DatabaseClient
{
    public static IDatabaseClient Create(Action<DatabaseClientOptions> configure)
    {
        var options = DatabaseClientOptions.Default;
        configure(options);
        var uri = new Uri(options.Url);
        
        return uri.Scheme switch
        {
            "http" or "https" => new WebDatabaseClient(options),
            "ws" or "wss" => throw new ArgumentException("ws:// is not yet supported"),
            "file" => throw new ArgumentException("file:// is not yet supported"),
            _ => throw new ArgumentException("Invalid scheme")
        };
    }
}