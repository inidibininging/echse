using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class ModifyAttributeExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        public AttributeExpression Property { get; set; }
        public AttributeExpression Attribute { get; set; }
        public SignConverterExpression SignConverter { get; set; }
        public NumberExpression Number { get; set; }
        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.Entity,
                LexiconSymbol.Letter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.Attribute,
                LexiconSymbol.Stats,
                LexiconSymbol.NegativeSign,
                LexiconSymbol.PositiveSign,
                LexiconSymbol.Number,
                LexiconSymbol.Position,
                LexiconSymbol.Scale,
                LexiconSymbol.Color,
                LexiconSymbol.Rotation,
                LexiconSymbol.Alpha                
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Gate
            if (machine.SharedContext.Current != LexiconSymbol.Modify)
                return;

            while (Identifier == null ||
                  Property == null || 
                  Attribute == null ||
                  SignConverter == null ||
                  Number == null)
            {

                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if (machine.SharedContext.Current == LexiconSymbol.EntityIdentifier && Identifier == null &&
                    Property != null &&
                    Attribute != null)
                {
                    // Console.WriteLine($"adding {nameof(EntityExpression)}");
                    Identifier = new EntityExpression();
                    Identifier.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier && Identifier == null &&
                    Property != null &&
                    Attribute != null)
                {
                    // Console.WriteLine($"adding {nameof(TagExpression)}");
                    Identifier = new TagExpression();
                    Identifier.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Attribute)
                {
                    // Console.WriteLine($"adding {nameof(AttributeExpression)}");
                    Attribute = new AttributeExpression();
                    Attribute.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.Position ||
                    machine.SharedContext.Current == LexiconSymbol.Scale ||
                    machine.SharedContext.Current == LexiconSymbol.Color ||
                    machine.SharedContext.Current == LexiconSymbol.Alpha ||
                    machine.SharedContext.Current == LexiconSymbol.Stats)
                {
                    // Console.WriteLine($"adding section {nameof(AttributeExpression)}");
                    Property = new AttributeExpression();
                    Property.Handle(machine);
                }
                
                //this part sets the order of the  
                if (machine.SharedContext.Current == LexiconSymbol.Letter && 
                    Identifier == null &&
                    Property != null &&
                    Attribute != null)
                {
                    // Console.WriteLine($"adding {nameof(IdentifierExpression)}");
                    Identifier = new IdentifierExpression();
                    Identifier.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.PositiveSign ||
                   machine.SharedContext.Current == LexiconSymbol.NegativeSign)
                {
                    // Console.WriteLine($"adding {nameof(SignConverterExpression)}");
                    SignConverter = new SignConverterExpression();
                    SignConverter.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.Number)
                {
                    // Console.WriteLine($"adding {nameof(NumberExpression)}");
                    Number = new NumberExpression();
                    Number.Handle(machine);
                }
            }

            if (Identifier == null)
            {
                throw new ArgumentNullException($"{nameof(Identifier)} not found. Maybe a syntax error? Ex. Mod Stats Health +123 myVar or Mod Stats Health +123 .myTag");
            }
            if (Property == null)
            {
                throw new ArgumentNullException($"{nameof(Property)} not found. Maybe a syntax error? Ex. Mod Stats Health +123 myVar or Mod Stats Health +123 .myTag");
            }
            if (Number == null)
            {
                throw new ArgumentNullException($"{nameof(Number)} not found. Maybe a syntax error? Ex. Mod Stats Health +123 myVar or Mod Stats Health +123 .myTag");
            }
        }
    }
}
