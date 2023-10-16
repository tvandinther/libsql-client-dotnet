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
        var url = ":memory:"u8;
        fixed (libsql_database_t* dbPtr = &_db)
        {
            fixed (byte* urlPtr = &url.GetPinnableReference())
            {
                var error = new Error();
                var errorCode = Libsql.libsql_open_ext(
                    urlPtr, 
                    dbPtr, 
                    &error.Ptr);
                
                error.ThrowIfNonZero(errorCode, "Failed to open database");
            }
        }
        
        fixed (libsql_connection_t* connectionPtr = &_connection)
        {
            var error = new Error();
            var errorCode = Libsql.libsql_connect(_db, connectionPtr, &error.Ptr);
            error.ThrowIfNonZero(errorCode, "Failed to connect to database");
        }
    }
    
    public unsafe Task<ResultSet> Execute(string sql)
    {
        return Task.Run(() =>
        {
            var error = new Error();
            
            var rows = new libsql_rows_t();
            var sqlPtr = sql.GetPtr();
            var errorCode = Libsql.libsql_execute(_connection, sqlPtr, &rows, &error.Ptr);
            Marshal.FreeHGlobal((IntPtr)sqlPtr);
            error.ThrowIfNonZero(errorCode, "Failed to execute query");
        
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