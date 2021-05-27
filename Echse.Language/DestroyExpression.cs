using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    /// <summary>
    /// Used for creating stuff and appending a unique id to 
    /// </summary>
    public class DestroyExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        //public DestroyerExpression Destroyer { get; set; }

        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.Destroy,
                LexiconSymbol.Destroyer,
                //LexiconSymbol.DestroyerLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Destroy)
                return;
            while (Identifier == null)
            {

                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                //if (machine.SharedContext.Current == LexiconSymbol.DestroyerLetter)
                //{
                //    Console.WriteLine($"adding {nameof(DestroyerExpression)}");
                //    Identifier = new DestroyerExpression();
                //    Identifier.Handle(machine);
                //}
                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier)
                {
                    Console.WriteLine($"adding {nameof(TagExpression)}");
                    Identifier = new TagExpression();
                    Identifier.Handle(machine);
                }
            }
            if (Identifier == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Identifier)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
