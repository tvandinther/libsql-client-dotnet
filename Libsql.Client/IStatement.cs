using System.Threading.Tasks;

namespace Libsql.Client
{
    /// <summary>
    /// Represents the result set of a SQL query.
    /// </summary>
    public interface IStatement
    {
        int ValuesBound { get; }
        void Bind(Integer integer);
        // void Bind(int integer);
        void Bind(Blob blob);
        void Bind(Real real);
        void Bind(Text text);
        void BindNull();
        Task<ulong> Execute();
        Task<IResultSet> Query();
    }
}
