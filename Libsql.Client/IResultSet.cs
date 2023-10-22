using System.Collections.Generic;

namespace Libsql.Client
{
    /// <summary>
    /// Represents the result set of a SQL query.
    /// </summary>
    public interface IResultSet
    {
        /// <summary>
        /// The ID of the last row inserted by the connection.
        /// </summary>
        /// <returns>The ID of the last row inserted.</returns>
        long LastInsertRowId { get; }
        
        /// <summary>
        /// The number of rows affected by the last query executed by the connection.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        ulong RowsAffected { get; }
        
        /// <summary>
        /// The names of the columns in the result set.
        /// </summary>
        /// <returns>An enumerable of column names.</returns>
        IEnumerable<string> Columns { get; }
        
        /// <summary>
        /// The rows in the result set.
        /// </summary>
        /// <returns>An enumerable of enumerable rows. Rows are enumerated by column.</returns>
        IEnumerable<IEnumerable<Value>> Rows { get; }
    }
}
