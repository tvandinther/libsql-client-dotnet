// using System.Threading.Tasks;

// namespace Libsql.Client
// {
//     internal class Statement : IStatement
//     {
//         internal readonly StatementWrapper _stmt;

//         public Statement(StatementWrapper statementWrapper)
//         {
//             _stmt = statementWrapper;
//         }

//         public void Bind(Integer integer)
//         {
//             _stmt.BindInt(1, integer.Value);
//         }

//         public void Bind(Blob blob)
//         {
//             throw new System.NotImplementedException();
//         }

//         public void Bind(Real real)
//         {
//             throw new System.NotImplementedException();
//         }

//         public void Bind(Text text)
//         {
//             throw new System.NotImplementedException();
//         }

//         public void BindNull()
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }