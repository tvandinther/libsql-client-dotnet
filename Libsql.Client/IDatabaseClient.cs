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
        /// <remarks>Use <see cref="Execute(string)"/> if your prepared statement does not return rows.</remarks>
        Task<IResultSet> Query(string sql);

        /// <summary>
        /// Executes the given SQL query with the specified parameters and returns the result set.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">The parameters to use in the query.</param>
        /// <returns>The result set returned by the query.</returns>
        /// <exception cref="LibsqlException">Thrown when the query fails to execute.</exception>
        /// <remarks>Use <see cref="Execute(string, object[])"/> if your prepared statement does not return rows.</remarks>
        Task<IResultSet> Query(string sql, params object[] args);

        /// <summary>
        /// Executes the given prepared statement and returns the result set.
        /// </summary>
        /// <param name="statement">The prepared statement to execute.</param>
        /// <returns>The result set returned by the prepared statement.</returns>
        /// <exception cref="LibsqlException">Thrown when the prepared statement fails to execute.</exception>
        /// <remarks>Use <see cref="Execute(IStatement)"/> if your prepared statement does not return rows.</remarks>
        Task<IResultSet> Query(IStatement statement);

        /// <summary>
        /// Executes the given SQL.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <returns>The number of affected rows.</returns>
        /// <exception cref="LibsqlException">Thrown when the SQL fails to execute.</exception>
        /// <remarks>Use <see cref="Query(string)"/> if your prepared statement returns rows.</remarks>
        Task<ulong> Execute(string sql);

        /// <summary>
        /// Executes the given SQL with the specified parameters.
        /// </summary>
        /// <param name="sql">The SQL query to execute.</param>
        /// <param name="args">The parameters to use in the query.</param>
        /// <returns>The number of affected rows.</returns>
        /// <exception cref="LibsqlException">Thrown when the SQL fails to execute.</exception>
        /// <remarks>Use <see cref="Query(string, object[])"/> if your prepared statement returns rows.</remarks>
        Task<ulong> Execute(string sql, params object[] args);

        /// <summary>
        /// Executes the given prepared statement.
        /// </summary>
        /// <param name="statement">The prepared statement to execute.</param>
        /// <returns>The number of affected rows.</returns>
        /// <exception cref="LibsqlException">Thrown when the prepared statement fails to execute.</exception>
        /// <remarks>Use <see cref="Query(IStatement)"/> if your prepared statement returns rows.</remarks>
        Task<ulong> Execute(IStatement statement);

        /// <summary>
        /// Prepares a statement with the database ready for values to be bound to it.
        /// </summary>
        /// <param name="sql">The SQL statement to prepare.</param>
        /// <returns>A statement to bind values to.</returns>
        Task<IStatement> Prepare(string sql);

        /// <summary>
        /// Synchronises the embedded replica database with the remote database.
        /// </summary>
        /// <exception cref="LibsqlException">Thrown when the synchronisation fails.</exception>
        Task Sync();
    }
}