namespace Echse.Language
{
    public abstract class NonTerminalExpression : AbstractLanguageExpression
    {
        public AbstractLanguageExpression NextExpression { get; set; }
    }
}
