namespace Echse.Domain
{
    /// <summary>
    /// Represents a variable of any type (not only Tags).
    /// </summary>
    public class TagVariable : Variable
    {
        public string Value { get; set; }
        public LexiconSymbol DataTypeSymbol { get; set; }

    }
}