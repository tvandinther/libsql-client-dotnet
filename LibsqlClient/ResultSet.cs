namespace LibsqlClient;

public record ResultSet(long LastInsertRowId, ulong RowsAffected, IEnumerable<string> Columns, IEnumerable<object[]> Rows);