﻿using System.Runtime.InteropServices;
using Bindings;
using LibsqlClient.Extensions;

namespace LibsqlClient;

internal class DatabaseWrapper : IDatabaseClient
{
    private libsql_database_t _db;
    private libsql_connection_t _connection;
    private readonly DatabaseType _type = DatabaseType.InMemory;

    public unsafe DatabaseWrapper(string url)
    {
        var error = new Error();
        int exitCode;
        
        fixed (libsql_database_t* dbPtr = &_db)
        {
            fixed (char* urlPtr = url)
            {
                exitCode = Libsql.libsql_open_ext(
                    (byte*)urlPtr, 
                    dbPtr, 
                    &error.Ptr);   
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
            exitCode = Libsql.libsql_connect(_db, connectionPtr, &error.Ptr);
        }
        
        error.ThrowIfNonZero(exitCode, "Failed to connect to database");
    }
    
    public unsafe Task<ResultSet> Execute(string sql)
    {
        return Task.Run(() =>
        {
            var error = new Error();
            var rows = new libsql_rows_t();
            int exitCode;
            
            fixed (char* sqlPtr = sql)
            {
                exitCode = Libsql.libsql_execute(_connection, (byte*)sqlPtr, &rows, &error.Ptr);
                Marshal.FreeHGlobal((IntPtr)sqlPtr);
            }
            
            error.ThrowIfNonZero(exitCode, "Failed to execute query");
            
            return new ResultSet(
                0,
                0,
                rows.GetColumnNames(),
                new Rows(rows)
            );
        });
    }

    public Task<ResultSet> Execute(string sql, params object[] args)
    {
        throw new NotImplementedException();
    }
}