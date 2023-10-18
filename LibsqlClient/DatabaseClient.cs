namespace LibsqlClient;

public static class DatabaseClient
{
    public static IDatabaseClient Create(Action<DatabaseClientOptions>? configure = default)
    {
        var options = DatabaseClientOptions.Default;
        configure?.Invoke(options);
        ArgumentNullException.ThrowIfNull(options.Url);
        
        return new DatabaseWrapper(options.Url);
        
        // if (IsInMemory(options.Url))
        // {
        //     return new DatabaseWrapper(":memory:");
        // }
        //
        // var uri = new Uri(options.Url);
        // return uri.Scheme switch
        // {
        //     "http" or "https" => throw new ArgumentException($"{uri.Scheme}:// is not yet supported"),
        //     "ws" or "wss" => throw new ArgumentException($"{uri.Scheme}:// is not yet supported"),
        //     "file" => throw new ArgumentException(options.Url),
        //     _ => throw new ArgumentException("Invalid scheme")
        // };
    }
    
    private static bool IsInMemory(string url) => url is "" or ":memory:";
}