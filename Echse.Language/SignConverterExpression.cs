using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class SignConverterExpression : TerminalExpression
    {
        public int Polarity { get; set; } = 0;

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current == LexiconSymbol.NegativeSign)
                Polarity = -1;
            if(machine.SharedContext.Current == LexiconSymbol.PositiveSign)
                Polarity = 1;
        }
    }
}
