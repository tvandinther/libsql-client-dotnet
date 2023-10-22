using System.Threading.Tasks;

namespace Libsql.Client
{
    /// <summary>
    /// Interface for a Libsql database client.
    /// </summary>
    public interface IDatabaseClient
    {
        /// <summary>
        /// Executes the given SQL query and returns the result set.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <returns>The result set returned by the query.</returns>
        Task<IResultSet> Execute(string sql);

        /// <summary>
        /// Executes the given SQL query with the specified parameters and returns the result set.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">The parameters to use in the query.</param>
        /// <returns>The result set returned by the query.</returns>
        Task<IResultSet> Execute(string sql, params object[] args);
    }
}