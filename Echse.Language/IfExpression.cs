using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class IfExpression : AbstractLanguageExpression
    {
        public VariableExpression Left { get; set; }
        public VariableExpression Right { get; set; }
        public ComparisonExpression Comparison { get; set; }

        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>()
        {
            LexiconSymbol.If,
            //LexiconSymbol.EndIf,
            LexiconSymbol.SkipMaterial,

            LexiconSymbol.Letter,
            LexiconSymbol.TagLetter,
            LexiconSymbol.TagIdentifier, 
            LexiconSymbol.RefIdentifier,
            //TODO: amplify with more comparers. This is the worst language ever -.-
            LexiconSymbol.Equal,
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.If)
                throw new InvalidOperationException("Trying to handle If, where If is not a token");


            while (Left == null ||
                  Right == null ||
                  Comparison == null)
            {

                if (!machine.SharedContext.MoveNext())
                    break;

                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left == null)
                {
                    Left = new IdentifierExpression();
                    Left.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Equal && Left != null)
                {
                    Comparison = new ComparisonExpression();
                    Comparison.Handle(machine);
                }

                //right depends on the type of the left identifier
                if (machine.SharedContext.Current != LexiconSymbol.SkipMaterial && 
                    machine.SharedContext.Current != LexiconSymbol.Equal &&
                    Left != null && 
                    Comparison != null)
		        {
		            if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier && Right == null) {
			            Right = new TagExpression();
			            Right.Handle(machine);
		            }

                    if (machine.SharedContext.Current == LexiconSymbol.RefIdentifier && Right == null) {
			            Right = new RefExpression();
			            Right.Handle(machine);
		            }

		            if (machine.SharedContext.Current == LexiconSymbol.Letter && Right == null) { 
		   	            Right = new IdentifierExpression();
			            Right.Handle(machine);
		            }
                }
            }

            if (Left == null)
            {
                throw new ArgumentNullException($"{nameof(Left)} not found. Maybe a syntax error?");
            }
            if (Right == null)
            {
                throw new ArgumentNullException($"{nameof(Right)} not found. Maybe a syntax error?");
            }
            if (Comparison == null)
            {
                throw new ArgumentNullException($"{nameof(Comparison)} not found. Maybe a syntax error?");
            }
        }
    }
}
