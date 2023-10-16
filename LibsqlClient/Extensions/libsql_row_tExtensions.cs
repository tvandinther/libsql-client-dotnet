using System.Runtime.InteropServices;
using Bindings;

namespace LibsqlClient.Extensions;

internal static class libsql_row_tExtensions
{
    public static Integer GetInteger(this libsql_row_t row, int columnIndex)
    {
        unsafe
        {
            var errorMessage = (byte**)0;
            long value;
            var exitCode = Libsql.libsql_get_int(row, columnIndex, &value, errorMessage);
            
            return new Integer((int)value);
        }
    }

    public static Text GetText(this libsql_row_t row, int columnIndex)
    {
        unsafe
        {
            var errorMessage = (byte**)0;
            var ptr = (byte*)0;
            var exitCode = Libsql.libsql_get_string(row, columnIndex, &ptr, errorMessage);
            
            var text = Marshal.PtrToStringAuto((IntPtr)ptr);
            Libsql.libsql_free_string(ptr);
            
            return new Text(text);
        }
    }

    public static Real GetReal(this libsql_row_t row, int columnIndex)
    {
        unsafe
        {
            var errorMessage = (byte**)0;
            double value;
            var exitCode = Libsql.libsql_get_float(row, columnIndex, &value, errorMessage);
            
            return new Real(value);
        }
    }

    public static Blob GetBlob(this libsql_row_t row, int columnIndex)
    {
        unsafe
        {
            var errorMessage = (byte**)0;
            var blob = new blob();
            var exitCode = Libsql.libsql_get_blob(row, columnIndex, &blob, errorMessage);
            var bytes = new byte[blob.len];
            Marshal.Copy((IntPtr) blob.ptr, bytes, 0, blob.len);
            Libsql.libsql_free_blob(blob);
            
            return new Blob(bytes);
        }
    }
}

