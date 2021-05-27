using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class WaitExpression : AbstractLanguageExpression
    {
        public NumberExpression Number { get; set; }
        public SignConverterExpression SignConverter { get; set; }
        public TimeUnitExpression Unit { get; set; }

        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.Milliseconds,
                LexiconSymbol.Seconds,
                LexiconSymbol.Hours,
                LexiconSymbol.Minutes,
                LexiconSymbol.NegativeSign,
                LexiconSymbol.PositiveSign,
                LexiconSymbol.Number,
        };
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Gate
            if(machine.SharedContext.Current != LexiconSymbol.Wait)
                return;

            while(Number == null ||
                  SignConverter == null ||
                  Unit == null){
                      if(!machine.SharedContext.MoveNext())
                        break;
                      if(!ValidLexemes.Contains(machine.SharedContext.Current))
                        continue;

                      // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                      if(
                        machine.SharedContext.Current == LexiconSymbol.Milliseconds ||
                        machine.SharedContext.Current == LexiconSymbol.Seconds ||
                        machine.SharedContext.Current == LexiconSymbol.Minutes ||
                        machine.SharedContext.Current == LexiconSymbol.Hours)
                      {
                        //Console.WriteLine($"adding {nameof(TimeUnitExpression)}");
                        Unit = new TimeUnitExpression();
                        Unit.Handle(machine);
                      }

                      if(machine.SharedContext.Current == LexiconSymbol.PositiveSign ||
                         machine.SharedContext.Current == LexiconSymbol.NegativeSign )
                      {
                        //Console.WriteLine($"adding {nameof(SignConverterExpression)}");
                        SignConverter = new SignConverterExpression();
                        SignConverter.Handle(machine);
                      }
                      if(machine.SharedContext.Current == LexiconSymbol.Number)
                      {
                        //Console.WriteLine($"adding {nameof(NumberExpression)}");
                          Number = new NumberExpression();
                          Number.Handle(machine);
                      }
                  }
            if (Number == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Number)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (SignConverter == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(SignConverter)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Unit == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Unit)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
