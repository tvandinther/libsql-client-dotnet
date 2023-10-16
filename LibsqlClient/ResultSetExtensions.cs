// using System.Runtime.InteropServices;
// using Bindings;
//
// namespace LibsqlClient;
//
// internal static class ResultSetExtensions
// {
//     internal static ResultSet ToManagedResultSet(this Bindings.ResultSet rs)
//     {
//         return new ResultSet(
//             rs.last_insert_rowid,
//             rs.rows_affected,
//             rs.GetColumns(),
//             rs.GetRows()
//         );
//     }
//     
//     private static unsafe string[] GetColumns(this Bindings.ResultSet resultSet)
//     {
//         var ptrs = resultSet.columns->AsSpan<nint>();
//         var arr = new string[ptrs.Length];
//         for (var i = 0; i < arr.Length; i++)
//         {
//             var stringBuffer = Marshal.PtrToStructure<ByteBuffer>(ptrs[i]);
//             arr[i] = stringBuffer.AsSpan<char>().ToString();
//             Libsql.byte_buffer_dealloc((ByteBuffer*)ptrs[i]);
//         }
//
//         resultSet.columns->ByteBufferDealloc();
//         return arr;
//     }
//     
//     private static unsafe IEnumerable<Value[]> GetRows(this Bindings.ResultSet resultSet)
//     {
//         var rowsIterator = resultSet.rows_iterator_ptr;
//         var hasNext = rowsIterator->RowsIteratorNext();
//         var rows = new List<Value[]>();
//         
//         while(hasNext)
//         {
//             var row = rowsIterator->RowsIteratorCurrent();
//             
//             var values = row->AsSpan<Bindings.Value>();
//             var rowValues = new Value[values.Length];
//             for (var i = 0; i < values.Length; i++)
//             {
//                 var value = values[i];
//                 rowValues[i] = value.value_type switch
//                 {
//                     Bindings.ValueType.Integer => new Integer(value.value_buffer->AsSpan<int>()[0]),
//                     Bindings.ValueType.Real => new Real(value.value_buffer->AsSpan<double>()[0]),
//                     Bindings.ValueType.Text => new Text(value.value_buffer->AsSpan<char>().ToString()),
//                     Bindings.ValueType.Blob => new Blob(value.value_buffer->AsSpan<byte>().ToArray()),
//                     Bindings.ValueType.Null => new Null(),
//                     _ => throw new ArgumentOutOfRangeException()
//                 };
//             }
//             rows.Add(rowValues);
//             hasNext = rowsIterator->RowsIteratorNext();
//         }
//         
//         rowsIterator->RowsIteratorDealloc();
//         return rows;
//     }
// }
//
