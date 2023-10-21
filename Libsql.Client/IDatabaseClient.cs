using System.Threading.Tasks;

namespace Libsql.Client
{
    public interface IDatabaseClient
    {
        Task<ResultSet> Execute(string sql);
        Task<ResultSet> Execute(string sql, params object[] args);
    }
}