using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Libsql.Client
{
    /// <summary>
    /// A prepared statement.
    /// </summary>
    public interface IStatement : IDisposable
    {
        /// <summary>
        /// Gets the number of values currently bound on the prepared statement.
        /// </summary>
        int BoundValuesCount { get; }

        /// <summary>
        /// Gets the number of parameters in the prepared statement.
        /// </summary>
        /// <remarks>
        /// If <c>?NNN</c> syntax is used, the Parameter count is the maximum value of 
        /// <c>NNN</c> used plus any unannotated positional parameters to the right.
        /// </remarks>
        int ParameterCount { get; }

        /// <summary>
        /// Binds a value to the prepared statement.
        /// </summary>
        /// <param name="integer">The <see cref="Integer"/> value to bind.</param>
        /// <param name="index">The positional index to bind the value to.</param>
        /// <remarks>The leftmost parameter has an index of <c>1</c>. 
        /// If <c>?NNN</c> syntax is used, the index is the value of <c>NNN</c>.</remarks>
        void Bind(Integer integer, int index);

        /// <inheritdoc cref="Bind"/>
        /// <param name="real">The <see cref="Real"/> value to bind.</param>
        void Bind(Real real, int index);

        /// <inheritdoc cref="Bind"/>
        /// <param name="text">The <see cref="Text"/> value to bind.</param>
        void Bind(Text text, int index);

        /// <inheritdoc cref="Bind"/>
        /// <param name="blob">The <see cref="Blob"/> value to bind.</param>
        void Bind(Blob blob, int index);

        /// <inheritdoc cref="Bind" path="/param[@name='index']"/>
        /// <summary>
        /// Binds a <see cref="Null"/> value to the prepared statement.
        /// </summary>
        void BindNull(int index);

        /// <inheritdoc cref="IDatabaseClient.Query(IStatement)"/>
        /// <seealso cref="IDatabaseClient.Execute(IStatement)">You may also call this method with the prepared statement as the argument.</seealso>
        /// <remarks>Use <see cref="Query"/> if your prepared statement returns rows.</remarks>
        Task<ulong> Execute();

        /// <inheritdoc cref="IDatabaseClient.Query(IStatement)"/>
        /// <seealso cref="IDatabaseClient.Query(IStatement)">You may also call this method with the prepared statement as the argument.</seealso>
        /// <remarks>Use <see cref="Execute"/> if your prepared statement does not return rows.</remarks>
        Task<IResultSet> Query();

        /// <summary>
        /// Resets the prepared statement so that it can be executed again.
        /// </summary>
        /// <remarks>Bound parameters are cleared.</remarks>
        void Reset();
    }
}
