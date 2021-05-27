using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language 
{
    public class IdentifierExpression : VariableExpression
    {

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();           
            
            while (machine.SharedContext.Current == LexiconSymbol.Letter) {
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",entityName.ToString());



            // Console.WriteLine($"Identifier set to {Name}");
        }
    }
}