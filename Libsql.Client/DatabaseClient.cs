using System;
using System.Threading.Tasks;

namespace Libsql.Client
{
    /// <summary>
    /// Provides a static method to create an instance of <see cref="IDatabaseClient"/>.
    /// </summary>
    public static class DatabaseClient
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IDatabaseClient"/> interface.
        /// </summary>
        /// <param name="configure">An optional action to configure the <see cref="DatabaseClientOptions"/>.</param>
        /// <returns>A new instance of the <see cref="IDatabaseClient"/> interface.</returns>
        /// <remarks>A client constitutes a connection to the database.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configure"/> is null.</exception>
        /// <exception cref="LibsqlException">Thrown when the database fails to open and/or connect.</exception>
        public static async Task<IDatabaseClient> Create(Action<DatabaseClientOptions> configure = default)
        {
            var options = DatabaseClientOptions.Default;
            configure?.Invoke(options);
            if (options.Url is null) throw new ArgumentNullException(nameof(options.Url));
        
            var DatabaseWrapper = new DatabaseWrapper(options);
            await DatabaseWrapper.Open();
            DatabaseWrapper.Connect();

            return DatabaseWrapper;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="IDatabaseClient"/> interface.
        /// </summary>
        /// <param name="path">The path to the database file.</param>
        /// <returns>A new instance of the <see cref="IDatabaseClient"/> interface.</returns>
        /// <remarks>An overload for opening a local database file. Equivalent to setting only the Url property of the options.</remarks>
        public static async Task<IDatabaseClient> Create(string path)
        {
            return await Create(opts => opts.Url = path);
        }
    }
}