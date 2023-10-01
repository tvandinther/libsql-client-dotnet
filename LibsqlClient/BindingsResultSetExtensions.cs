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
            Libsql.byte_buffer_dealloc((ByteBuffer*)ptrs[i]);
        }

        resultSet.columns->ByteBufferDealloc();
        return arr;
    }

    public static unsafe IEnumerable<object?[]> GetRows(this Bindings.ResultSet resultSet)
    {
        var rowsIterator = resultSet.rows_iterator_ptr;
        var hasNext = rowsIterator->RowsIteratorNext();
        var rows = new List<object?[]>();
        
        while(hasNext)
        {
            var row = rowsIterator->RowsIteratorCurrent();
            
            var values = row->AsSpan<Value>();
            var rowValues = new object?[values.Length];
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                rowValues[i] = (value.value_type switch
                {
                    Bindings.ValueType.Integer => value.value_buffer->AsSpan<int>()[0],
                    Bindings.ValueType.Real => value.value_buffer->AsSpan<double>()[0],
                    Bindings.ValueType.Text => value.value_buffer->AsSpan<char>().ToString(),
                    Bindings.ValueType.Blob => value.value_buffer->AsSpan<byte>().ToArray(),
                    Bindings.ValueType.Null => null,
                    _ => throw new ArgumentOutOfRangeException()
                });
            }
            rows.Add(rowValues);
            hasNext = rowsIterator->RowsIteratorNext();
        }

        // rowsIterator->
        rowsIterator->RowsIteratorDealloc();
        return rows;
    }
}

