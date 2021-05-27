using System;
using System.Collections.Generic;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class GroupArgumentExpression<T> : AbstractLanguageExpression
        where T : VariableExpression, new()
    {
        public List<T> Arguments { get; set; } = new List<T>();
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while(machine.SharedContext.Current == LexiconSymbol.GroupBegin || 
                  machine.SharedContext.Current == LexiconSymbol.GroupEnd || 
                  machine.SharedContext.Current == LexiconSymbol.Letter ||
                  machine.SharedContext.Current == LexiconSymbol.Identifier ||
                  machine.SharedContext.Current == LexiconSymbol.ArgumentSeparator || 
                  machine.SharedContext.Current == LexiconSymbol.SkipMaterial ||
                  machine.SharedContext.Current == LexiconSymbol.NotFound)
            {
                
                if (machine.SharedContext.Current == LexiconSymbol.Letter)
                {
                    // Console.WriteLine($"adding {nameof(T)} to group expression");
                    Arguments.Add(new T());
                    Arguments.Last().Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.GroupEnd)
                    break;
                
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            if (Arguments.Count() == 0)
                throw new InvalidOperationException($"Syntax error: ${nameof(Arguments)} side near {machine.SharedContext.CurrentBuffer}. No arguments provided");
        }
    }
}