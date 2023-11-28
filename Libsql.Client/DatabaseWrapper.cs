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
        private readonly DatabaseClientOptions _options;
        private readonly DatabaseType _type;

        public unsafe DatabaseWrapper(DatabaseClientOptions options)
        {
            Debug.Assert(options.Url != null, "url is null");

            _options = options;

            if (options.Url == "" || options.Url == ":memory:")
            {
                _type = DatabaseType.Memory;
            }
            else
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
                            _type = _options.ReplicaPath != null ? DatabaseType.EmbeddedReplica : DatabaseType.Remote;
                            break;
                        default:
                            throw new InvalidOperationException($"Unsupported scheme: {uri.Scheme}");
                    }
                }
                catch (UriFormatException)
                {
                    _type = DatabaseType.File;
                }
            }
        }

        internal async Task Open()
        {
            var error = new Error();
            int exitCode;

            switch (_type)
            {
                case DatabaseType.Memory:
                case DatabaseType.File:
                    exitCode = OpenMemoryOrFileDatabase(_options, error);
                    break;
                case DatabaseType.Remote:
                case DatabaseType.EmbeddedReplica:
                    exitCode = await Task.Run(() => OpenRemoteDatabase(_options, error));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported database type: {_type}");
            }
        
            error.ThrowIfNonZero(exitCode, "Failed to open database");
        }

        private unsafe int OpenMemoryOrFileDatabase(DatabaseClientOptions options, Error error)
        {
            // C# empty strings have null pointers, so we need to give the url some meat.
            var url = options.Url is "" ? "\0" : options.Url;
            
            fixed (libsql_database_t* dbPtr = &_db)
            {
                fixed (byte* urlPtr = Encoding.UTF8.GetBytes(url))
                {
                    return Bindings.libsql_open_ext(urlPtr, dbPtr, &error.Ptr);   
                }
            }
        }

        private unsafe int OpenRemoteDatabase(DatabaseClientOptions options, Error error)
        {
            var url = options.Url;
            var authToken = options.AuthToken;
            var replicaPath = options.ReplicaPath;
            var useHttps = options.UseHttps;
            
            fixed (libsql_database_t* dbPtr = &_db)
            {
                fixed (byte* urlPtr = Encoding.UTF8.GetBytes(url))
                {
                    fixed (byte* authTokenPtr = Encoding.UTF8.GetBytes(authToken))
                    {
                        if (replicaPath is null)
                        {
                            return Bindings.libsql_open_remote(urlPtr, authTokenPtr, dbPtr, &error.Ptr);
                        }

                        fixed (byte* replicaPathPtr = Encoding.UTF8.GetBytes(replicaPath))
                        {
                            return Bindings.libsql_open_sync(
                                replicaPathPtr,
                                urlPtr,
                                authTokenPtr,
                                dbPtr,
                                &error.Ptr
                            );
                        }
                    }
                }
            }
        }

        internal unsafe void Connect()
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

        public async Task Sync()
        {
            if (_type != DatabaseType.EmbeddedReplica)
            {
                throw new InvalidOperationException("Cannot sync a non-replica database");
            }
            
            await Task.Run(() =>
            {
                unsafe
                {
                    var error = new Error();
                    int exitCode;
            
                    exitCode = Bindings.libsql_sync(_db, &error.Ptr);
            
                    error.ThrowIfNonZero(exitCode, "Failed to sync database");
                }
            });
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