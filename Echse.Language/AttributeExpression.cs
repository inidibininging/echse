
using System;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class AttributeExpression : TerminalExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current == LexiconSymbol.Position ||
                    machine.SharedContext.Current == LexiconSymbol.Scale ||
                    machine.SharedContext.Current == LexiconSymbol.Color ||
                    machine.SharedContext.Current == LexiconSymbol.Alpha ||
                    machine.SharedContext.Current == LexiconSymbol.Attribute ||
                    machine.SharedContext.Current == LexiconSymbol.Stats)
                Name = machine.SharedContext.CurrentBuffer;
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
