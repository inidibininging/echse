using System;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class DataTypeExpression : TerminalExpression
    {
        public LexiconSymbol DataType { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while (DataType == LexiconSymbol._0)
            {
                if (machine.SharedContext.Current == LexiconSymbol.TagDataType ||
                    machine.SharedContext.Current == LexiconSymbol.NumberDataType ||
                    machine.SharedContext.Current == LexiconSymbol.RefDataType)
                {
                    DataType = machine.SharedContext.Current;
                }

                if (!machine.SharedContext.MoveNext())
                    break;
            }
            if (DataType == LexiconSymbol._0)
                throw new InvalidOperationException($"Syntax error: DataType cannot be determined near '{machine.SharedContext.CurrentBuffer}'");
        }
    }
}
