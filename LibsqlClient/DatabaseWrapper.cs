using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Bindings;
using LibsqlClient.Extensions;

namespace LibsqlClient;

internal class DatabaseWrapper : IDatabaseClient
{
    private libsql_database_t _db;
    private libsql_connection_t _connection;
    private readonly DatabaseType _type = DatabaseType.InMemory;

    public unsafe DatabaseWrapper()
    {
        var url = "::memory::"u8;
        var errorMessage = (byte**)0;
        fixed (libsql_database_t* dbPtr = &_db)
        {
            var errorCode = Libsql.libsql_open_ext(
                (byte*)MemoryMarshal.GetReference(url), 
                dbPtr, 
                errorMessage);
        }
        
        fixed (libsql_connection_t* connectionPtr = &_connection)
        {
            var errorCode = Libsql.libsql_connect(_db, connectionPtr, errorMessage);
        }
    }
    
    public unsafe Task<ResultSet> Execute(string sql)
    {
        return Task.Run(() =>
        {
            var errorMessage = (byte**)0;
            ReadOnlySpan<Byte> sqlSpan = Encoding.UTF8.GetBytes(sql);
            var sqlBytePtr = (byte*) MemoryMarshal.GetReference(sqlSpan);
            var rows = new libsql_rows_t();
            var errorCode = Libsql.libsql_execute(_connection, sqlBytePtr, &rows, errorMessage);
            
            var rs = new ResultSet(
                0,
                0,
                rows.GetColumnNames(),
                new Rows(rows)
            );
            
            return rs;
        });
    }

    public Task<ResultSet> Execute(string sql, params object[] args)
    {
        throw new NotImplementedException();
    }
}