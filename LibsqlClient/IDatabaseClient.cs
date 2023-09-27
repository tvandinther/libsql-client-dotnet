namespace LibsqlClient;

public interface IDatabaseClient
{
    public Task<ResultSet> Execute(string sql);
    public Task<ResultSet> Execute(string sql, params object[] args);
}