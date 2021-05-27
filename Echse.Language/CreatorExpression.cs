using System;
using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class CreatorExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while (machine.SharedContext.Current == LexiconSymbol.CreatorLetter)
            {
                if (machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());                
                if (!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("", entityName.ToString());
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
