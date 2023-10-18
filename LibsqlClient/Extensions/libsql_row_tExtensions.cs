using System.Runtime.InteropServices;
using Bindings;

namespace LibsqlClient.Extensions;

internal static class libsql_row_tExtensions
{
    public static unsafe Integer GetInteger(this libsql_row_t row, int columnIndex)
    {
        var error = new Error();
        long value;
        var exitCode = Libsql.libsql_get_int(row, columnIndex, &value, &error.Ptr);
            
        error.ThrowIfNonZero(exitCode, "Failed to get INTEGER");
            
        return new Integer((int)value);
    }

    public static unsafe Text GetText(this libsql_row_t row, int columnIndex)
    {
        var error = new Error();
        var ptr = (byte*)0;
        var exitCode = Libsql.libsql_get_string(row, columnIndex, &ptr, &error.Ptr);
            
        error.ThrowIfNonZero(exitCode, "Failed to get TEXT");
            
        var text = Marshal.PtrToStringAuto((IntPtr)ptr);
        Libsql.libsql_free_string(ptr);

        if (text is null)
        {
            throw new InvalidOperationException("Text was marshalled to null");   
        }
            
        return new Text(text);
    }

    public static unsafe Real GetReal(this libsql_row_t row, int columnIndex)
    {
        var error = new Error();
        double value;
        var exitCode = Libsql.libsql_get_float(row, columnIndex, &value, &error.Ptr);
            
        error.ThrowIfNonZero(exitCode, "Failed to get REAL");
            
        return new Real(value);
    }

    public static unsafe Blob GetBlob(this libsql_row_t row, int columnIndex)
    {
        var error = new Error();
        var blob = new blob();
        var exitCode = Libsql.libsql_get_blob(row, columnIndex, &blob, &error.Ptr);
            
        error.ThrowIfNonZero(exitCode, "Failed to get BLOB");
            
        var bytes = new byte[blob.len];
        Marshal.Copy((IntPtr) blob.ptr, bytes, 0, blob.len);
        Libsql.libsql_free_blob(blob);
            
        return new Blob(bytes);
    }
}
