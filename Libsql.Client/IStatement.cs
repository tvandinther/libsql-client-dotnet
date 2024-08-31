using System.Threading.Tasks;

namespace Libsql.Client
{
    /// <summary>
    /// Represents the result set of a SQL query.
    /// </summary>
    public interface IStatement
    {
        int BoundValuesCount { get; }
        void Bind(Integer integer);
        
        void Bind(Real real);
        void Bind(Text text);
        void Bind(Blob blob);
        void BindNull();
        Task<ulong> Execute();
        Task<IResultSet> Query();
    }
}
