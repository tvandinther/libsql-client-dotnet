using System.Runtime.InteropServices;
using Bindings;

namespace LibsqlClient.Extensions;

internal static class libsql_rows_tExtensions
{
    public static IEnumerable<string> GetColumnNames(this libsql_rows_t libsqlRowsT)
    {
        unsafe
        {
            var columnCount = Libsql.libsql_column_count(libsqlRowsT);
            var columnNames = new string[columnCount];
            
            for (var i = 0; i < columnCount; i++)
            {
                var errorMessage = (byte**)0;
                var ptr = (byte*)0;
                var columnName = Libsql.libsql_column_name(libsqlRowsT, i, &ptr, errorMessage);
                
                var text = Marshal.PtrToStringAuto((IntPtr)ptr);
                Libsql.libsql_free_string(ptr);
                
                columnNames[i] = text;
            }
            
            return columnNames;
        }
    }
}

