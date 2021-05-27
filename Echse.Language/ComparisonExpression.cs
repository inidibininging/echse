using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Echse.Domain;

namespace Echse.Language
{
    public class ComparisonExpression : AbstractLanguageExpression
    {
        public LexiconSymbol ComparisonSymbol { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Equal)
                throw new NotImplementedException("No implementation found for Equal");
            ComparisonSymbol = LexiconSymbol.Equal;
        }
    }
}
