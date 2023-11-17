using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Libsql.Client.Extensions;

namespace Libsql.Client
{
    internal class DatabaseWrapper : IDatabaseClient, IDisposable
    {
        private libsql_database_t _db;
        private libsql_connection_t _connection;

        public unsafe DatabaseWrapper(DatabaseClientOptions options)
        {
            Debug.Assert(options.Url != null, "url is null");

            if (!(options.Url == "" || options.Url == ":memory:"))
            {
                try
                {
                    var uri = new Uri(options.Url);
                    switch (uri.Scheme)
                    {
                        case "http":
                        case "https":
                        case "ws":
                        case "wss":
                            throw new LibsqlException($"{uri.Scheme}:// is not yet supported");
                    }
                }
                catch (UriFormatException) { }
            }

            // C# empty strings have null pointers, so we need to give the url some meat.
            var url = options.Url is "" ? "\0" : options.Url;

            var error = new Error();
            int exitCode;
        
            fixed (libsql_database_t* dbPtr = &_db)
            {
                fixed (byte* urlPtr = Encoding.UTF8.GetBytes(url))
                {
                    exitCode = Bindings.libsql_open_ext(urlPtr, dbPtr, &error.Ptr);   
                }
            }
        
            error.ThrowIfNonZero(exitCode, "Failed to open database");
        
            Connect();
        }

        private unsafe void Connect()
        {
            var error = new Error();
            int exitCode;
        
            fixed (libsql_connection_t* connectionPtr = &_connection)
            {
                exitCode = Bindings.libsql_connect(_db, connectionPtr, &error.Ptr);
            }
        
            error.ThrowIfNonZero(exitCode, "Failed to connect to database");
        }
    
        public async Task<IResultSet> Execute(string sql)
        {
            return await Task.Run(() =>
            {
                unsafe
                {
                    var error = new Error();
                    var rows = new libsql_rows_t();
                    int exitCode;
            
                    fixed (byte* sqlPtr = Encoding.UTF8.GetBytes(sql))
                    {
                        exitCode = Bindings.libsql_execute(_connection, sqlPtr, &rows, &error.Ptr);
                    }
            
                    error.ThrowIfNonZero(exitCode, "Failed to execute query");
            
                    return new ResultSet(
                        Bindings.libsql_last_insert_rowid(_connection),
                        Bindings.libsql_changes(_connection),
                        rows.GetColumnNames(),
                        new Rows(rows)
                    );   
                }
            });
        }

        public Task<IResultSet> Execute(string sql, params object[] args)
        {
            throw new NotImplementedException();
        }

        private void ReleaseUnmanagedResources()
        {
            Bindings.libsql_disconnect(_connection);
            Bindings.libsql_close(_db);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DatabaseWrapper()
        {
            ReleaseUnmanagedResources();
        }
    }
}