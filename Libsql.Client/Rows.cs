using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Libsql.Client.Extensions;

namespace Libsql.Client
{
    internal class Rows : IEnumerable<IEnumerable<Value>>
    {
        private readonly libsql_rows_t _libsqlRowsT;
        private RowEnumeratorData _enumeratorData = new RowEnumeratorData();
        private int? ColumnCount => _enumeratorData.ColumnTypes?.Length ?? null;

        internal Rows(libsql_rows_t libsqlRowsT)
        {
            _libsqlRowsT = libsqlRowsT;
        }
    
        public IEnumerator<IEnumerable<Value>> GetEnumerator() => 
            new RowsEnumerator(_libsqlRowsT, ref _enumeratorData);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class RowsEnumerator : IEnumerator<IEnumerable<Value>>
    {
        private readonly libsql_rows_t _libsqlRowsT;
        private int _currentIndex = -1;
        private int ParsedRowsCount => _enumeratorData.ParsedRows.Count;
        private readonly RowEnumeratorData _enumeratorData;

        public RowsEnumerator(libsql_rows_t libsqlRowsT, ref RowEnumeratorData enumeratorData)
        {
            _libsqlRowsT = libsqlRowsT;
            _enumeratorData = enumeratorData;
            if (_enumeratorData.ParsedRows is null) _enumeratorData.ParsedRows = new List<IEnumerable<Value>>();

            if (_enumeratorData.ColumnTypes != null) return;

            unsafe
            {
                var columnCount = Bindings.libsql_column_count(_libsqlRowsT);
                _enumeratorData.ColumnTypes = new ValueType[columnCount];
            
                PopulateColumnTypes(columnCount);
            }
        
            Debug.Assert(_enumeratorData.ParsedRows != null, "_enumeratorData.ParsedRows is null");
            Debug.Assert(_enumeratorData.ColumnTypes != null, "_enumeratorData.ColumnTypes is null");
        }

        private unsafe void PopulateColumnTypes(int columnCount) {
            // Must fetch the cursor before we can read the column types
            var firstRow = GetRow();

            // Do not populate column types if there is no first row
            if ((int) firstRow.ptr == 0) return;

            for (var i = 0; i < columnCount; i++)
            {
                int columnType;
                var error = new Error();
                Debug.Assert(firstRow.ptr != null, "firstRow is null. Can not find column type on a null pointer.");
                var errorCode = Bindings.libsql_column_type(_libsqlRowsT, firstRow, i, &columnType, &error.Ptr);
                error.ThrowIfNonZero(errorCode, "Failed to get column type");
                _enumeratorData.ColumnTypes[i] = (ValueType)columnType;
            }

            // Parse the first row so that it is cached now that the cursor has moved on to the next row
            ParseRow(firstRow);
        }
    
        public bool MoveNext()
        {
            _currentIndex++;
        
            if (ParsedRowsCount > _currentIndex)
            {
                Current = _enumeratorData.ParsedRows[_currentIndex];
            
                return true;
            }

            if (ParsedRowsCount == _currentIndex && _enumeratorData.FullyParsed) return false;
            
            var row = GetRow();
            Current = ParseRow(row);
            
            return Current != null;
        }

        private unsafe libsql_row_t GetRow()
        {
            var row = new libsql_row_t();
            var error = new Error();
            var exitCode = Bindings.libsql_next_row(_libsqlRowsT, &row, &error.Ptr);
        
            error.ThrowIfNonZero(exitCode, "Failed to get next row");

            return row;
        }

        private unsafe Value[] ParseRow(libsql_row_t row)
        {
            var parsedRow = new Value[_enumeratorData.ColumnTypes.Length];
        
            if (row.ptr == null)
            {
                _enumeratorData.FullyParsed = true;
                Bindings.libsql_free_rows(_libsqlRowsT);
                return null;
            }
        
            for (var i = 0; i < _enumeratorData.ColumnTypes.Length; i++)
            {
                var value = 
                    _enumeratorData.ColumnTypes[i] == ValueType.Integer ? row.GetInteger(i) :
                    _enumeratorData.ColumnTypes[i] == ValueType.Real ? row.GetReal(i) :
                    _enumeratorData.ColumnTypes[i] == ValueType.Text ? row.GetText(i) :
                    _enumeratorData.ColumnTypes[i] == ValueType.Blob ? row.GetBlob(i) :
                    _enumeratorData.ColumnTypes[i] == ValueType.Null ? (Value)new Null() :
                    throw new ArgumentOutOfRangeException($"Non-exhaustive check. Could not find a case to match value of {_enumeratorData.ColumnTypes[i]}");
            
                parsedRow[i] = value;
            }
        
            Bindings.libsql_free_row(row);
            _enumeratorData.ParsedRows.Add(parsedRow);

            return parsedRow;
        }

        public void Reset() => _currentIndex = -1;

        public IEnumerable<Value> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }

    internal class RowEnumeratorData
    {
        public IList<IEnumerable<Value>> ParsedRows;
        public ValueType[] ColumnTypes;
        public bool FullyParsed;
    }

    internal enum ValueType {
        Integer = 1,
        Real,
        Text,
        Blob,
        Null,
    }
}
