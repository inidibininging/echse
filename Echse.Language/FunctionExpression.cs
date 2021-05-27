using System;
using System.Linq;
using System.Text;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class FunctionExpression : VariableExpression
    {
        public GroupArgumentExpression<IdentifierExpression> Arguments { get; private set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var functionName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.FunctionIdentifier || machine.SharedContext.Current == LexiconSymbol.FunctionLetter){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    functionName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",functionName.ToString().Skip(1));
            // Console.WriteLine($"function set to {Name}");
            
            Arguments = new GroupArgumentExpression<IdentifierExpression>();
            // Console.WriteLine($"group expression found");
            Arguments.Handle(machine);

            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Arguments == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Arguments)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}