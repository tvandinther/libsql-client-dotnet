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
        /// <remarks>
        /// Supported values:
        /// <list type="bullet">
        ///   <item>
        ///     <description><c>":memory:"</c> or <c>""</c> - Creates an in-memory database.</description>
        ///   </item>
        ///   <item>
        ///     <description>Local file path (e.g. <c>file:localdb.sqlite</c> or <c>C:\data\localdbs.sqlite</c>) - Creates or opens a file-based database.</description>
        ///   </item>
        ///   <item>
        ///     <description>Remote URL (e.g. <c>http://example.com/db</c>) - For remote database support.</description>
        ///   </item>
        /// </list>
        /// Default: <c>""</c>.
        /// </remarks>
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
