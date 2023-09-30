using Bindings;

namespace LibsqlClient;

internal class DatabaseWrapper : IDatabaseClient
{
    private readonly uint _key;
    private readonly DatabaseType _type = DatabaseType.InMemory;

    public DatabaseWrapper()
    {
        _key = Libsql.database_new_in_memory();
        Console.WriteLine(_key);
    }
    
    public Task<ResultSet> Execute(string sql)
    {
        throw new NotImplementedException();
    }

    public Task<ResultSet> Execute(string sql, params object[] args)
    {
        throw new NotImplementedException();
    }
}