using System;
using System.Collections.Generic;

namespace Libsql.Client
{
    public class ResultSet : IEquatable<ResultSet>
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

        public bool Equals(ResultSet other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return LastInsertRowId == other.LastInsertRowId && RowsAffected == other.RowsAffected &&
                   Equals(Columns, other.Columns) && Equals(Rows, other.Rows);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ResultSet)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = LastInsertRowId.GetHashCode();
                hashCode = (hashCode * 397) ^ RowsAffected.GetHashCode();
                hashCode = (hashCode * 397) ^ (Columns != null ? Columns.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Rows != null ? Rows.GetHashCode() : 0);
                return hashCode;
            }
        }
    }   
}