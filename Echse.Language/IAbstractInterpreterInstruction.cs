using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public interface IAbstractInterpreterInstruction<out TExpr> : IState<string, IEchseContext>
        where TExpr : class, IAbstractLanguageExpression
    {
        int FunctionIndex { get; }
        FunctionInstruction Scope { get; }
        // int ScopeIndex { get; }
        TExpr Expression { get; }
    }
}