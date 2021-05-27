using System;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class TimeUnitExpression : TerminalExpression
    {
        
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
             if(machine.SharedContext.Current == LexiconSymbol.Milliseconds ||
                machine.SharedContext.Current == LexiconSymbol.Seconds ||
                machine.SharedContext.Current == LexiconSymbol.Minutes ||
                machine.SharedContext.Current == LexiconSymbol.Hours)
            {
                Name = machine.SharedContext.CurrentBuffer;
            }
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
        
    }
}
