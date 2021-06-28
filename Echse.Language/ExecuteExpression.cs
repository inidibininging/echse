using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class ExecuteExpression : VariableExpression
    {
        public GroupArgumentExpression<IdentifierExpression> Arguments { get; private set; }
        
        
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var functionName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.Execute || machine.SharedContext.Current == LexiconSymbol.ExecuteLetter) {
                if(machine.SharedContext.CurrentBufferRaw.Count > 0)
                    functionName.Append(machine.SharedContext.CurrentBufferRaw.Last());
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("", functionName.ToString().Skip(1));
            // Console.WriteLine($"function set to {Name}");
            
            Arguments = new GroupArgumentExpression<IdentifierExpression>();
            // Console.WriteLine($"group expression found");
            Arguments.Handle(machine);
        }
    }
}
