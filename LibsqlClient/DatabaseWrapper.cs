using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Bindings;

namespace LibsqlClient;

internal class DatabaseWrapper : IDatabaseClient
{
    private readonly int _key;
    private readonly DatabaseType _type = DatabaseType.InMemory;

    public DatabaseWrapper()
    {
        _key = Libsql.database_new_in_memory();
        Libsql.database_open_connection(_key);
        Console.WriteLine("Created database with key: {0}", _key);
    }
    
    public Task<ResultSet> Execute(string sql)
    {
        unsafe
        {
            fixed (char* ptr = sql)
            {
                var p = (ushort*)ptr;
                return Task.Run(() =>
                {
                    var rs = Libsql.database_query(_key, p, sql.Length);

                    return new ResultSet(
                        rs.last_insert_rowid,
                        rs.rows_affected,
                        rs.GetColumns(),
                        rs.GetRows()
                        );
                });
            }
        }
    }

    public Task<ResultSet> Execute(string sql, params object[] args)
    {
        throw new NotImplementedException();
    }
}