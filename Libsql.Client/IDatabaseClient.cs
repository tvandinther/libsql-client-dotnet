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
        /// <exception cref="LibsqlException">Thrown when the query fails to execute.</exception>
        Task<IResultSet> Query(string sql);

        /// <summary>
        /// Executes the given SQL query with the specified parameters and returns the result set.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">The parameters to use in the query.</param>
        /// <returns>The result set returned by the query.</returns>
        /// <exception cref="LibsqlException">Thrown when the query fails to execute.</exception>
        Task<IResultSet> Query(string sql, params object[] args);

        /// <summary>
        /// Executes the given SQL.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <exception cref="LibsqlException">Thrown when the SQL fails to execute.</exception>
        Task<ulong> Execute(string sql);

        /// <summary>
        /// Executes the given SQL with the specified parameters.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">The parameters to use in the query.</param>
        /// <exception cref="LibsqlException">Thrown when the SQL fails to execute.</exception>
        Task<ulong> Execute(string sql, params object[] args);

        /// <summary>
        /// Synchronises the embedded replica database with the remote database.
        /// </summary>
        /// <exception cref="LibsqlException">Thrown when the synchronisation fails.</exception>
        Task Sync();
    }
}