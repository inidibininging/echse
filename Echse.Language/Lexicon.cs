using System.Collections.Generic;
using System.Linq;
using Echse.Domain;

namespace Echse.Language
{
    public class Lexicon
    {
        public readonly List<char> Empty = new () { char.MinValue };
        public readonly List<char> Space = new (){ ' ' };
        public readonly List<char> Separator = System.Environment.NewLine.ToList();
        public readonly List<char> Carriage = new () { '\r' };
        public readonly List<char> LineFeed = new () { '\n' };
        public readonly List<char> Tab = new () { '\t' };
        public readonly List<char> PositiveSign = new () { '+' };
        public readonly List<char> NegativeSign = new () { '-' };
        public readonly List<char> Assignment = new () { '=' };
        public readonly List<char> If = "If".ToList();
        public readonly List<char> Equal = "Is".ToList();
        public readonly List<char> EndIf = "EndIf".ToList();
        public readonly List<char> Modify = "Mod".ToList();
        public readonly List<char> Set = "Set".ToList();
        public readonly List<char> TagDataType = "Tag".ToList();
        public readonly List<char> RefDataType = "Ref".ToList();
        public readonly List<char> NumberDataType = "Number".ToList();
        public readonly List<char> Attribute = "Attribute".ToList();
        public readonly List<char> Stats = "Stats".ToList();
        public readonly List<char> Position = "Position".ToList();
        public readonly List<char> Rotation = "Rotation".ToList();
        public readonly List<char> Scale = "Scale".ToList();
        public readonly List<char> Alpha = "Alpha".ToList();
        public readonly List<char> Color = "Color".ToList();
        public readonly List<char> ExecuteAttribute = "!".ToList();
        public readonly List<char> CreateAttribute = "@>".ToList();
        public readonly List<char> DestroyAttribute = "<@".ToList();
        // public readonly List<char> ClassNameForAttributes = typeof(CharacterSheet).FullName.ToList();
        public readonly List<char> EntityIdentifier = "#".ToList();
        public readonly List<char> RefIdentifier = "&".ToList();
        public readonly List<char> TagIdentifier = "'".ToList();
        public readonly List<char> FunctionIdentifier = ":".ToList();
        public readonly List<char> MillisecondsAttribute = "Milliseconds".ToList();
        public readonly List<char> SecondsAttribute = "Seconds".ToList();
        public readonly List<char> MinutesAttribute = "Minutes".ToList();
        public readonly List<char> HoursAttribute = "Hours".ToList();
        public readonly List<char> WaitAttribute = "Wait".ToList();
        public readonly List<char> EveryAttribute = "Every".ToList();
        public readonly List<char> XAttribute = "X".ToList();
        public readonly List<char> YAttribute = "Y".ToList();
        public readonly List<char> GroupBegin = "(".ToList();
        public readonly List<char> GroupEnd = ")".ToList();
        public readonly List<char> ArgumentSeparator = ",".ToList();
        public readonly List<char> Return = "Return".ToList();


        private readonly Dictionary<List<char>,LexiconSymbol> SymbolTable = new();

        private void InitializeSymbolTable()
        {
            SymbolTable.Clear();
            SymbolTable.Add(Space,LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Separator,LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Empty, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Carriage, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(LineFeed, LexiconSymbol.SkipMaterial);
            SymbolTable.Add(Tab, LexiconSymbol.SkipMaterial);

            SymbolTable.Add(PositiveSign,LexiconSymbol.PositiveSign);
            SymbolTable.Add(NegativeSign,LexiconSymbol.NegativeSign);
            SymbolTable.Add(Assignment, LexiconSymbol.Assign);
            SymbolTable.Add(Equal, LexiconSymbol.Equal);

            SymbolTable.Add(If, LexiconSymbol.If);
            SymbolTable.Add(EndIf, LexiconSymbol.EndIf);
            
            SymbolTable.Add(TagDataType, LexiconSymbol.TagDataType);
            SymbolTable.Add(RefDataType, LexiconSymbol.RefDataType);
            SymbolTable.Add(NumberDataType, LexiconSymbol.NumberDataType);

            SymbolTable.Add(GroupBegin,LexiconSymbol.GroupBegin);
            SymbolTable.Add(GroupEnd,LexiconSymbol.GroupEnd);
            SymbolTable.Add(ArgumentSeparator,LexiconSymbol.ArgumentSeparator);
            SymbolTable.Add(Modify,LexiconSymbol.Modify);
            SymbolTable.Add(Set, LexiconSymbol.Set);

            SymbolTable.Add(Scale,LexiconSymbol.Scale);
            SymbolTable.Add(Position,LexiconSymbol.Position);
            SymbolTable.Add(Rotation,LexiconSymbol.Rotation);
            SymbolTable.Add(Color,LexiconSymbol.Color);
            SymbolTable.Add(Stats, LexiconSymbol.Stats);

            SymbolTable.Add(CreateAttribute, LexiconSymbol.Create);
            SymbolTable.Add(DestroyAttribute,LexiconSymbol.Destroy);

            // SymbolTable.Add(ClassNameForAttributes,LexiconSymbol.Entity);
            SymbolTable.Add(EntityIdentifier,LexiconSymbol.EntityIdentifier);
            SymbolTable.Add(TagIdentifier,LexiconSymbol.TagIdentifier);
            SymbolTable.Add(RefIdentifier, LexiconSymbol.RefIdentifier);
            SymbolTable.Add(FunctionIdentifier,LexiconSymbol.FunctionIdentifier);
            SymbolTable.Add(ExecuteAttribute,LexiconSymbol.Execute);
            SymbolTable.Add(MillisecondsAttribute,LexiconSymbol.Milliseconds);
            SymbolTable.Add(SecondsAttribute,LexiconSymbol.Seconds);
            SymbolTable.Add(MinutesAttribute,LexiconSymbol.Minutes);
            SymbolTable.Add(HoursAttribute,LexiconSymbol.Hours);
            SymbolTable.Add(WaitAttribute,LexiconSymbol.Wait);
            SymbolTable.Add(EveryAttribute,LexiconSymbol.Every);
            SymbolTable.Add(XAttribute, LexiconSymbol.Attribute);
            SymbolTable.Add(YAttribute, LexiconSymbol.Attribute);
            SymbolTable.Add(Return, LexiconSymbol.Return);
            
        }
        public Lexicon()
        {
            InitializeSymbolTable();
        }

        public LexiconSymbol FindLexiconSymbol(char token)
        {
            if (char.IsLetter(token))
                return LexiconSymbol.Letter;
            if (char.IsDigit(token))
                return LexiconSymbol.Digit;
            var possibleResult = SymbolTable.FirstOrDefault(symbolKeyPair => symbolKeyPair.Key.Count == 1 &&
                                                                             symbolKeyPair.Key.ElementAt(0).Equals(token));
            if (possibleResult.Key == null)
            {                
                return LexiconSymbol.NotFound;
            }                
            else
                return possibleResult.Value;
        }

        public LexiconSymbol FindLexiconSymbol(List<char> token)
        {
            var possibleResult =  SymbolTable
                    .Where(symbolKeyPair => symbolKeyPair.Key.Count == token.Count &&
                                            symbolKeyPair.Key.SequenceEqual(token));
            if (!possibleResult.Any())
            {                
                return LexiconSymbol.NotFound;
            }                
            else
                return possibleResult.First().Value;
        }
    }
}
