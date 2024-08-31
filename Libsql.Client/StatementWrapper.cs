using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Libsql.Client
{
    internal class StatementWrapper : IStatement
    {
        int IStatement.ValuesBound => _bindIndex - 1;
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
                foreach (var arg in values)
                {
                    switch (arg)
                    {
                        case int val:
                            BindInt(val);
                            break;
                        case double d:
                            BindFloat(d);
                            break;
                        case string s:
                            BindString(s);
                            break;
                        case byte[] b:
                            BindBlob(b);
                            break;
                        case null:
                            Bind();
                            break;
                        default:
                            throw new ArgumentException($"Unsupported argument type: {arg.GetType()}");
                    }
                }
            }
        }

        private unsafe void BindInt(int value)
        {
            var error = new Error();
            var index = _bindIndex;
            Console.WriteLine($"Binding int at index {index}");
            var exitCode = Bindings.libsql_bind_int(Stmt, index, value, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            _bindIndex++;
        }

        private unsafe void BindFloat(double value)
        {
            var error = new Error();
            var index = _bindIndex;
            Console.WriteLine($"Binding int at index {index}");
            var exitCode = Bindings.libsql_bind_float(Stmt, index, value, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            _bindIndex++;
        }

        private unsafe void BindString(string value)
        {
            var error = new Error();
            var index = _bindIndex;
            Console.WriteLine($"Binding int at index {index}");
            fixed (byte* sPtr = Encoding.UTF8.GetBytes(value)) {
                var exitCode = Bindings.libsql_bind_string(Stmt, index, sPtr, &error.Ptr);

                error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            }
            _bindIndex++;
        }

        private unsafe void BindBlob(byte[] value)
        {
            var error = new Error();
            var index = _bindIndex;
            Console.WriteLine($"Binding int at index {index}");
            fixed (byte* bPtr = value) {
                var exitCode = Bindings.libsql_bind_blob(Stmt, index, bPtr, value.Length, &error.Ptr);

                error.ThrowIfNonZero(exitCode, $"Failed to bind integer at index {index} with value {value}");
            }
            _bindIndex++;
        }

        private unsafe void Bind()
        {
            var error = new Error();
            var index = _bindIndex;
            Console.WriteLine($"Binding int at index {index}");
            var exitCode = Bindings.libsql_bind_null(Stmt, index, &error.Ptr);

            error.ThrowIfNonZero(exitCode, $"Failed to bind null at index {index}");
            _bindIndex++;
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

        public void Bind(Integer integer)
        {
            BindInt(integer.Value);
        }

        public void Bind(Real real)
        {
            BindFloat(real.Value);
        }

        public void Bind(Text text)
        {
            throw new NotImplementedException();
        }

        public void Bind(Blob blob)
        {
            throw new NotImplementedException();
        }

        public void BindNull()
        {
            throw new NotImplementedException();
        }

        public Task<ulong> Execute()
        {
            return _database.Execute(this);
        }

        public Task<IResultSet> Query()
        {
            return _database.Query(this);
        }

        ~StatementWrapper()
        {
            ReleaseUnmanagedResources();
        }
    }
}
