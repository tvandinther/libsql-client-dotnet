using System;
using System.Text;

namespace Libsql.Client
{
    internal class Statement
    {
        public libsql_stmt_t Stmt;

        public unsafe Statement(string sql)
        {
            Stmt = new libsql_stmt_t();
            var error = new Error();
            int exitCode;

            fixed (byte* sqlPtr = Encoding.UTF8.GetBytes(sql))
            {
                fixed (libsql_stmt_t* statementPtr = &Stmt)
                {
                    exitCode = Bindings.libsql_prepare(sqlPtr, statementPtr, &error.Ptr);
                }
            }

            error.ThrowIfNonZero(exitCode, $"Failed to prepare statement for: {sql}");
        }

        public unsafe void Bind(object[] values)
        {
            var error = new Error();
            int exitCode;

            if (values is null)
            {
                exitCode = Bindings.libsql_bind_null(Stmt, 1, &error.Ptr);
                error.ThrowIfNonZero(exitCode, "Failed to bind null parameter");
            }
            else 
            {
                for (var i = 0; i < values.Length; i++)
                {
                    var arg = values[i];

                    
                    if (arg is int val) {
                        exitCode = Bindings.libsql_bind_int(Stmt, i + 1, val, &error.Ptr);
                    }
                    else if (arg is double d) {
                        exitCode = Bindings.libsql_bind_float(Stmt, i + 1, d, &error.Ptr);
                    }
                    else if (arg is string s) {
                        fixed (byte* sPtr = Encoding.UTF8.GetBytes(s))
                        {
                            exitCode = Bindings.libsql_bind_string(Stmt, i + 1, sPtr, &error.Ptr);
                        }
                    }
                    else if (arg is byte[] b) {
                        fixed (byte* bPtr = b)
                        {
                            exitCode = Bindings.libsql_bind_blob(Stmt, i + 1, bPtr, b.Length, &error.Ptr);
                        }
                    }
                    else if (arg is null)
                    {
                        exitCode = Bindings.libsql_bind_null(Stmt, i + 1, &error.Ptr);
                    }
                    else
                    {
                        throw new ArgumentException($"Unsupported argument type: {arg.GetType()}");
                    }
                    
                    error.ThrowIfNonZero(exitCode, $"Failed to bind parameter. Type: {(arg is null ? "null" : arg.GetType().ToString())} Value: {arg}");
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Bindings.libsql_free_stmt(Stmt);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~Statement()
        {
            ReleaseUnmanagedResources();
        }
    }
}
