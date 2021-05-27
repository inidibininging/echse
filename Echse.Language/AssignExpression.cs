
using System;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class AssignExpression : VariableExpression
    {
        public DataTypeExpression DataType { get; set; }
        public VariableExpression Left { get; private set; }
        public VariableExpression Right { get; private set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while(
                  machine.SharedContext.Current == LexiconSymbol.Set || 
                  machine.SharedContext.Current == LexiconSymbol.Letter || 
                  machine.SharedContext.Current == LexiconSymbol.Identifier || 
                  machine.SharedContext.Current == LexiconSymbol.Assign ||
                  
                  machine.SharedContext.Current == LexiconSymbol.TagIdentifier ||
                  machine.SharedContext.Current == LexiconSymbol.TagLetter ||
                  machine.SharedContext.Current == LexiconSymbol.TagDataType ||
                  
                  machine.SharedContext.Current == LexiconSymbol.RefIdentifier ||
                  machine.SharedContext.Current == LexiconSymbol.RefLetter ||
                  machine.SharedContext.Current == LexiconSymbol.RefDataType ||
                  
                  machine.SharedContext.Current == LexiconSymbol.Execute ||
                  machine.SharedContext.Current == LexiconSymbol.ExecuteLetter ||

                  machine.SharedContext.Current == LexiconSymbol.NumberDataType ||
                  machine.SharedContext.Current == LexiconSymbol.SkipMaterial ) {

                //This is for now the way I can verify if DataType is not set
                if (DataType == null) {
                    DataType = new DataTypeExpression();
                    DataType.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left == null)
                {
                    Left = new IdentifierExpression();
                    Left.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier 
                    && Left != null && DataType?.DataType == LexiconSymbol.TagDataType)
                {
                    Right = new TagExpression();
                    Right.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.RefIdentifier 
                    && Left != null && DataType?.DataType == LexiconSymbol.RefDataType)
                {
                    Right = new RefExpression();
                    Right.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.Execute 
                    && Left != null && DataType?.DataType == LexiconSymbol.TagDataType)
                {
                    Right = new ExecuteExpression();
                    Right.Handle(machine);
                }
                
                if (machine.SharedContext.Current == LexiconSymbol.Letter && Left != null)
                {
                    Right = new IdentifierExpression();
                    Right.Handle(machine);
                }

                if(!machine.SharedContext.MoveNext() || (Left != null && Right != null))
                    break;
            }
            if (Left == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Left)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Right == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Right)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (DataType == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(DataType)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}