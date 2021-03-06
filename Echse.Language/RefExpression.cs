using System;
using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class RefExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.RefIdentifier || 
                  machine.SharedContext.Current == LexiconSymbol.RefLetter)
            {
                if(machine.SharedContext.CurrentBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrentBufferRaw.Last());
                // Console.WriteLine("ok faction");
                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",entityName.ToString().Skip(1));
            // Console.WriteLine($"Faction set to {Name}");
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException($"Syntax Error. Cannot process Ref Expression Name near {machine.SharedContext.CurrentBuffer}");
        }
    }    
}
