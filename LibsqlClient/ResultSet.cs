namespace LibsqlClient;

public record ResultSet(int LastInsertRowId, int RowsAffected, IEnumerable<string> Columns, IEnumerable<object[]> Rows);