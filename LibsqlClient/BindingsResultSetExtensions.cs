using System.Runtime.InteropServices;
using Bindings;

namespace LibsqlClient;

internal static class BindingsResultSetExtensions
{
    public static unsafe string[] GetColumns(this Bindings.ResultSet resultSet)
    {
        var ptrs = resultSet.columns->AsSpan<nint>();
        var arr = new string[ptrs.Length];
        for (var i = 0; i < arr.Length; i++)
        {
            var stringBuffer = Marshal.PtrToStructure<ByteBuffer>(ptrs[i]);
            arr[i] = stringBuffer.AsSpan<char>().ToString();
            Libsql.free_byte_buffer((ByteBuffer*)ptrs[i]);
        }

        Libsql.free_byte_buffer(resultSet.columns);
        return arr;
    }
}

