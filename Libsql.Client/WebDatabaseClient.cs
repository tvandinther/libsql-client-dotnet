namespace Libsql.Client;

internal class WebDatabaseClient : IDatabaseClient
{
    public WebDatabaseClient(DatabaseClientOptions options)
    {
        
    }

    public async Task<ResultSet> Execute(string sql)
    {
        throw new NotImplementedException();
    }

    public async Task<ResultSet> Execute(string sql, params object[] args)
    {
        throw new NotImplementedException();
    }
}

