namespace Libsql.Client;

public class DatabaseClientOptions
{
    private DatabaseClientOptions(string url, string? authToken = null, bool useHttps = false)
    {
        Url = url;
        AuthToken = authToken;
        UseHttps = useHttps;
    }

    internal static DatabaseClientOptions Default => new("");
    public string Url { get; set; }
    public string? AuthToken { get; set; }
    public bool UseHttps { get; set; }

    public void Deconstruct(out string url, out string? token, out bool useHttps)
    {
        url = Url;
        token = AuthToken;
        useHttps = UseHttps;
    }
}

