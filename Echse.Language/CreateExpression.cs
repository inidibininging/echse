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
        public List<VariableExpression> Identifiers { get; set; } = new();
        public CreatorExpression Creator { get; set; }

        private List<LexiconSymbol> ValidLexemes { get; } = new List<LexiconSymbol>() {
                LexiconSymbol.CreatorIdentifier,
                LexiconSymbol.CreatorLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter,
                LexiconSymbol.Letter,
                LexiconSymbol.SkipMaterial
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Create)
                return;
            // at this point, one creation identifier was found
            while ((Identifiers == null ||
                    Identifiers.Count == 0 ||
                    ValidLexemes.Contains(machine.SharedContext.Current) ||
                    Creator == null))
            {
                if (!machine.SharedContext.MoveNext())
                    break;
                
                if (machine.SharedContext.Current == LexiconSymbol.Create)
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
                    var identifier = new TagExpression();
                    identifier.Handle(machine);
                    Identifiers.Add(identifier);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Letter)
                {
                    // Console.WriteLine($"adding {nameof(IdentifierExpression)}");
                    var identifier = new IdentifierExpression();
                    identifier.Handle(machine);
                    Identifiers.Add(identifier);
                }
            }
            if (Creator == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Creator)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Identifiers.Count == 0)
                throw new InvalidOperationException($"Syntax error: ${nameof(Identifiers)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
