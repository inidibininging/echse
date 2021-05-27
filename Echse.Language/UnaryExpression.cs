using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public abstract class UnaryExpression<T> : NonTerminalExpression
        where T : AbstractLanguageExpression
    {
        public T NextExpr { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            NextExpr.Handle(machine);
        }
    }
}
