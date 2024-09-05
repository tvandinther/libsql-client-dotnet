using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Libsql.Client.Extensions;

namespace Libsql.Client
{
    internal class StatementWrapper : IStatement
    {
        int IStatement.BoundValuesCount => _bindIndex - 1;

        public int ParameterCount => GetParameterCount();

        private int _bindIndex = 1;
        public readonly libsql_stmt_t Stmt;
        private readonly libsql_connection_t _connection;
        private readonly DatabaseWrapper _database;

        public unsafe StatementWrapper(DatabaseWrapper database, libsql_connection_t connection, string sql)
        {
            _database = database;
            _connection = connection;
            Stmt = new libsql_stmt_t();
            var error = new Error();
            int exitCode;

            fixed (byte* sqlPtr = Encoding.UTF8.GetBytes(sql))
            {
                fixed (libsql_stmt_t* statementPtr = &Stmt)
                {
                    exitCode = Bindings.libsql_prepare(_connection, sqlPtr, statementPtr, &error.Ptr);
                }
            }

            error.ThrowIfNonZero(exitCode, $"Failed to prepare statement for: {sql}");
            
            Debug.Assert(_database != null);
        }

        public unsafe void BindAll(object[] values)
        {
            if (values is null)
            {
                var error = new Error();
                var exitCode = Bindings.libsql_bind_null(Stmt, 1, &error.Ptr);
                error.ThrowIfNonZero(exitCode, "Failed to bind null parameters");
            }
            else 
            {
                for (int i = 1; i <= values.Length; i++)
                {
                    object arg = values[i - 1];
                    switch (arg)
                    {
                        case int val:
                            BindInt(val, i);
                            break;
                        case double d:
                            BindFloat(d, i);
                            break;
                        case string s:
                            BindString(s, i);
                            break;
                        case byte[] b:
                            BindBlob(b, i);
                            break;
                        case null:
                            Bind(i);
                            break;
                        default:
                            throw new ArgumentException($"Unsupported argument type: {arg.GetType()}");
                    }
                }
            }
        }

        private unsafe void BindInt(int value, int index)
        {
            var error = new Error();
            var exitCode = Bindings.libsql_bind_int(Stmt, index, value, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            _bindIndex++;
        }

        private unsafe void BindFloat(double value, int index)
        {
            var error = new Error();
            var exitCode = Bindings.libsql_bind_float(Stmt, index, value, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            _bindIndex++;
        }

        private unsafe void BindString(string value, int index)
        {
            var error = new Error();
            fixed (byte* sPtr = Encoding.UTF8.GetBytes(value)) {
                var exitCode = Bindings.libsql_bind_string(Stmt, index, sPtr, &error.Ptr);

                error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            }
            _bindIndex++;
        }

        private unsafe void BindBlob(byte[] value, int index)
        {
            var error = new Error();
            fixed (byte* bPtr = value) {
                var exitCode = Bindings.libsql_bind_blob(Stmt, index, bPtr, value.Length, &error.Ptr);

                error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            }
            _bindIndex++;
        }

        private unsafe void Bind(int index)
        {
            var error = new Error();
            var exitCode = Bindings.libsql_bind_null(Stmt, index, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind null at index {index}");
            _bindIndex++;
        }

        private unsafe int GetParameterCount()
        {
            var error = new Error();
            int count;
            var exitCode = Bindings.libsql_stmt_parameter_count(Stmt, &count,&error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to get parameter count");

            return count;
        }

        public string GetParameterNameAtIndex(int index)
        {
            unsafe {
                var error = new Error();
                var ptr = (byte*)0;
                var exitCode = Bindings.libsql_stmt_parameter_name(Stmt, index, &ptr, &error.Ptr);

                error.ThrowIfNonZero(exitCode, "Failed to get parameter name");

                var text = CustomMarshal.PtrToStringUTF8((IntPtr)ptr);
                Bindings.libsql_free_string(ptr);

                if (text is null)
                {
                    throw new InvalidOperationException("Text was marshalled to null");   
                }

                return text.Substring(1);
            }
        }

        public void Bind(Integer integer, int index)
        {
            BindInt(integer.Value, index);
        }

        public void Bind(Real real, int index)
        {
            BindFloat(real.Value, index);
        }

        public void Bind(Text text, int index)
        {
            BindString(text.Value, index);
        }

        public void Bind(Blob blob, int index)
        {
            BindBlob(blob.Value, index);
        }

        public void BindNull(int index)
        {
            Bind(index);
        }

        public Task<ulong> Execute()
        {
            return _database.Execute(this);
        }

        public Task<IResultSet> Query()
        {
            return _database.Query(this);
        }

        public void Reset()
        {
            var error = new Error();
            int exitCode;

            unsafe {
                exitCode = Bindings.libsql_reset_stmt(Stmt, &error.Ptr);
            }

            _bindIndex = 1;

            error.ThrowIfNonZero(exitCode, "Failed to reset statement");
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

        ~StatementWrapper()
        {
            ReleaseUnmanagedResources();
        }
    }
}
