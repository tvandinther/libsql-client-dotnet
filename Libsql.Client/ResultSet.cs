using System.Collections.Generic;

namespace Libsql.Client
{
    internal class ResultSet : IResultSet
    {
        public long LastInsertRowId { get; }
        
        public ulong RowsAffected { get; }

        public IEnumerable<string> Columns { get; }

        public IEnumerable<IEnumerable<Value>> Rows { get; }

        public ResultSet(long lastInsertRowId, ulong rowsAffected, IEnumerable<string> columns, IEnumerable<IEnumerable<Value>> rows)
        {
            LastInsertRowId = lastInsertRowId;
            RowsAffected = rowsAffected;
            Columns = columns;
            Rows = rows;
        }
    }
}