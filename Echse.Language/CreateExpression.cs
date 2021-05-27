using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    /// <summary>
    /// Used for creating stuff and appending a unique id to 
    /// </summary>
    public class CreateExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        public CreatorExpression Creator { get; set; }

        private List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.CreatorIdentifier,
                LexiconSymbol.CreatorLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter,
                LexiconSymbol.Letter
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Create)
                return;
            while (Identifier == null || Creator == null)
            {
                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                if (machine.SharedContext.Current == LexiconSymbol.CreatorLetter)
                {
                    // Console.WriteLine($"adding {nameof(CreatorExpression)}");
                    Creator = new CreatorExpression();
                    Creator.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier)
                {
                    // Console.WriteLine($"adding {nameof(TagExpression)}");
                    Identifier = new TagExpression();
                    Identifier.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Letter)
                {
                    // Console.WriteLine($"adding {nameof(IdentifierExpression)}");
                    Identifier = new IdentifierExpression();
                    Identifier.Handle(machine);
                }
            }
            if (Creator == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Creator)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Identifier == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Identifier)} side is not implemented near {machine.SharedContext.CurrentBuffer}");            
        }
    }
}
