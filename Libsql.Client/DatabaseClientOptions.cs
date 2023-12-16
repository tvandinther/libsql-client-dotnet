namespace Libsql.Client
{
    /// <summary>
    /// Represents the options for configuring <see cref="IDatabaseClient"/>.
    /// </summary>
    public class DatabaseClientOptions
    {
        private DatabaseClientOptions(string url, string authToken = null, string replicaPath = null, bool useHttps = true)
        {
            Url = url;
            AuthToken = authToken;
            ReplicaPath = replicaPath;
            UseHttps = useHttps;
        }

        internal static DatabaseClientOptions Default => new DatabaseClientOptions("");
        
        /// <summary>
        /// Gets or sets the URL of the database server.
        /// </summary>
        /// <remarks>Default: <c>""</c>. <c>""</c> or <c>":memory:"</c> will create an in-memory database.</remarks>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the authentication token used to connect to the database.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Gets or sets the path to the replica database file.
        /// </summary>
        /// <remarks>Default: <c>null</c>. If set, the database will be replicated to the specified file.</remarks>
        public string ReplicaPath { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to use HTTPS protocol for database connections.
        /// </summary>
        /// <remarks>Default: <c>true</c>.</remarks>
        public bool UseHttps { get; set; }
    }
}
