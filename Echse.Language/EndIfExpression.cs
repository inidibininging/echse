using System;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class EndIfExpression : AbstractLanguageExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current != LexiconSymbol.EndIf)
                throw new InvalidOperationException($"Syntax Error: EndIf not found in {machine.SharedContext.CurrentBuffer}");
        }
    }
}
