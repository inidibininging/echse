using System;

namespace Echse.Language
{
    public abstract class BinaryExpression<T1,T2> : NonTerminalExpression
        where T1 : AbstractLanguageExpression
        where T2 : AbstractLanguageExpression
    {
        public T1 Left { get; set; }
        public T2 Right { get; set; }
    }
}
