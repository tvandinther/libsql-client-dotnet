using System.Collections;
using Bindings;
using LibsqlClient.Extensions;

namespace LibsqlClient;

internal class Rows : IEnumerable<IEnumerable<Value>>
{
    private readonly libsql_rows_t _libsqlRowsT;

    internal Rows(libsql_rows_t libsqlRowsT)
    {
        _libsqlRowsT = libsqlRowsT;
    }
    
    public IEnumerator<IEnumerable<Value>> GetEnumerator()
    {
        return new RowsEnumerator(_libsqlRowsT);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal class RowsEnumerator : IEnumerator<IEnumerable<Value>>
{
    private readonly libsql_rows_t _libsqlRowsT;
    private readonly ValueType[] _columnTypes;

    public RowsEnumerator(libsql_rows_t libsqlRowsT)
    {
        _libsqlRowsT = libsqlRowsT;

        unsafe
        {
           var columnCount = Libsql.libsql_column_count(_libsqlRowsT);
           _columnTypes = new ValueType[columnCount];
           for (var i = 0; i < columnCount; i++)
           {
               int type;
               var errorMessage = (byte**)0;
               var columnType = Libsql.libsql_column_type(_libsqlRowsT, i, &type, errorMessage);
               _columnTypes[i] = (ValueType)columnType;
           } 
        }
    }
    
    public bool MoveNext()
    {
        unsafe
        {
            var row = new libsql_row_t();
            var errorMessage = (byte**)0;
            var parsedRow = new Value[_columnTypes.Length];
            var exitCode = Libsql.libsql_next_row(_libsqlRowsT, &row, errorMessage);
            
            if (row.ptr is null)
            {
                return false;
            }
            
            for (var i = 0; i < _columnTypes.Length; i++)
            {
                Value value = _columnTypes[i] switch
                {
                    ValueType.Integer => row.GetInteger(i),
                    ValueType.Real => row.GetReal(i),
                    ValueType.Text => row.GetText(i),
                    ValueType.Blob => row.GetBlob(i),
                    ValueType.Null => new Null(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                parsedRow[i] = value;
            }
            
            Libsql.libsql_free_row(row);
            Current = parsedRow;
            
            return true;
        }
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Value> Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        Libsql.libsql_free_rows(_libsqlRowsT);
    }
}

internal enum ValueType {
    Integer,
    Real,
    Text,
    Blob,
    Null,
}
