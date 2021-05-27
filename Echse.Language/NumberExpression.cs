using System;
using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class NumberExpression : TerminalExpression
    {
        public int? NumberValue { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.Number){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            NumberValue = int.Parse(entityName.ToString());
            
            if(!NumberValue.HasValue)                
                throw new InvalidOperationException($"Syntax error: ${nameof(NumberValue)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
