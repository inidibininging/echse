using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public abstract class AbstractLanguageExpression : IAbstractLanguageExpression
    {
        public abstract void Handle(IStateMachine<string, Tokenizer> machine);
    }
}
