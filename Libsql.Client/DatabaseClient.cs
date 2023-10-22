using System;

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
        public static IDatabaseClient Create(Action<DatabaseClientOptions> configure = default)
        {
            var options = DatabaseClientOptions.Default;
            configure?.Invoke(options);
            if (options.Url is null) throw new ArgumentNullException(nameof(options.Url));
        
            return new DatabaseWrapper(options);
        }
    }
}