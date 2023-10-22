using System;

namespace Libsql.Client
{
    /// <summary>
    /// Represents an exception that is thrown when an error occurs in the Libsql.Client library.
    /// </summary>
    public class LibsqlException : ApplicationException
    {
        internal LibsqlException()
        {
        }

        internal LibsqlException(string message)
            : base(message)
        {
        }

        internal LibsqlException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
