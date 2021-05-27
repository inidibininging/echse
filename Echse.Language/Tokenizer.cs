using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Echse.Domain;

namespace Echse.Language
{
    public class Tokenizer : IEnumerator<LexiconSymbol>
    {
        private Lexicon LanguageTokens { get; set; } = new Lexicon();
        private List<char> CurrentTokenBuffer { get; set; }
        public ReadOnlyCollection<char> CurrenBufferRaw => CurrentTokenBuffer.AsReadOnly();
        public string CurrentBuffer => string.Join("", CurrentTokenBuffer);

        private LexiconSymbol CurrentSymbol {get; set; }
        public LexiconSymbol Current => CurrentSymbol;

        object IEnumerator.Current => CurrentSymbol;

        private StreamReader TokenStreamReader { get; set; }
        public Tokenizer(StreamReader tokenStreamReader)
        {
            TokenStreamReader = tokenStreamReader ?? throw new ArgumentNullException(nameof(tokenStreamReader));
            CurrentSymbol = LexiconSymbol.NotFound;
        }
        private void InitlializeTokenBufferIfEmpty(){
            if(CurrentTokenBuffer == null)
                CurrentTokenBuffer = new List<char>();
        }

        private readonly List<Func<LexiconSymbol, 
            LexiconSymbol, 
            LexiconSymbol,
            LexiconSymbol>> _rules = new()
        {
            (singleToken, nowToken, beforeToken) => nowToken == LexiconSymbol.Set ?
                                                    LexiconSymbol.Set :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.Equal ?
                                                    LexiconSymbol.Equal :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.Letter &&
                                                    nowToken == LexiconSymbol.StringScope ? 
                                                    LexiconSymbol.StringCharacter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.StringScope &&
                                                        nowToken == LexiconSymbol.StringCharacter ? 
                                                    LexiconSymbol.StringScope :
                                                    nowToken,



            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.Letter &&
                                                    (nowToken == LexiconSymbol.NotFound || nowToken == LexiconSymbol.NA)?
                                                    LexiconSymbol.Letter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.Letter &&
                                                    nowToken == LexiconSymbol.NotFound ? 
                                                    LexiconSymbol.Letter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => singleToken == LexiconSymbol.Equal &&
                                                    nowToken == LexiconSymbol.NotFound ? 
                                                    LexiconSymbol.Assign :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter &&
                                                    nowToken == LexiconSymbol.NotFound ? 
                                                    LexiconSymbol.Identifier :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.FunctionIdentifier &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.FunctionLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.Execute &&
                                                    //(nowToken == LexiconSymbol.Letter || nowToken == LexiconSymbol.NotFound) &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.ExecuteLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.ExecuteLetter &&
                                                    //(nowToken == LexiconSymbol.Letter || nowToken == LexiconSymbol.NotFound) &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.ExecuteLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.FunctionIdentifier &&
                                                    (nowToken == LexiconSymbol.Letter || nowToken == LexiconSymbol.NA) &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.FunctionLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.FunctionLetter &&
                                                    (nowToken == LexiconSymbol.Letter || nowToken == LexiconSymbol.NA) &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.FunctionLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.RefIdentifier &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.RefLetter :
                                                    nowToken,
            
            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.RefLetter &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.RefLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.TagIdentifier &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.TagLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.TagLetter &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    singleToken == LexiconSymbol.Letter ?
                                                    LexiconSymbol.TagLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.EntityIdentifier &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    (singleToken == LexiconSymbol.Letter || singleToken == LexiconSymbol.Digit) ? 
                                                    LexiconSymbol.EntityLetter :
                                                    nowToken,
            
            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.EntityLetter &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    (singleToken == LexiconSymbol.Letter || singleToken == LexiconSymbol.Digit) ?
                                                    LexiconSymbol.EntityLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.Create &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    (singleToken == LexiconSymbol.Letter || singleToken == LexiconSymbol.Digit) ?
                                                    LexiconSymbol.CreatorLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.CreatorLetter &&
                                                    nowToken == LexiconSymbol.Letter &&
                                                    (singleToken == LexiconSymbol.Letter || singleToken == LexiconSymbol.Digit) ?
                                                    LexiconSymbol.CreatorLetter :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => beforeToken == LexiconSymbol.Number &&
                                                    nowToken == LexiconSymbol.NotFound &&
                                                    singleToken == LexiconSymbol.Digit ?
                                                    LexiconSymbol.Number :
                                                    nowToken,

            (singleToken, nowToken, beforeToken) => (beforeToken == LexiconSymbol.PositiveSign || beforeToken == LexiconSymbol.NegativeSign) &&
                                                    nowToken == LexiconSymbol.NotFound &&
                                                    singleToken == LexiconSymbol.Digit ?
                                                    LexiconSymbol.Number :
                                                    nowToken


        };
        private List<Func<LexiconSymbol, LexiconSymbol, LexiconSymbol, LexiconSymbol>> Rules
        {
            get
            {
                return _rules;
            }
        }

        public bool MoveNext()
        {
            InitlializeTokenBufferIfEmpty();
            if(CurrentSymbol == LexiconSymbol.SkipMaterial ||
               CurrentSymbol == LexiconSymbol.NA ){
                   CurrentTokenBuffer.Clear();
            }

            while(!TokenStreamReader.EndOfStream){
                var rawCharacter = TokenStreamReader.Read();
                var convertedCharacter = Convert.ToChar(rawCharacter);
                var singleToken = LanguageTokens.FindLexiconSymbol(convertedCharacter);

                CurrentTokenBuffer.Add(convertedCharacter);

                var validTokenBuffer = LanguageTokens.FindLexiconSymbol(CurrentTokenBuffer);

                var tempValidTokenBuffer = validTokenBuffer;
                var rulesToApply = validTokenBuffer;
                rulesToApply = Rules
                    .Select(appliedRule => rulesToApply = appliedRule(singleToken, rulesToApply, CurrentSymbol))
                    .LastOrDefault(ls => ls != LexiconSymbol.NA);
                if (rulesToApply != tempValidTokenBuffer)
                {
                    validTokenBuffer = rulesToApply;
                }

                if (CurrentTokenBuffer.Count > 0 && validTokenBuffer == LexiconSymbol.Assign)
                    CurrentTokenBuffer.RemoveAt(CurrentTokenBuffer.Count - 1);

                CurrentSymbol = validTokenBuffer;

                var mayStopSymbol = LanguageTokens.FindLexiconSymbol(new List<char>() { convertedCharacter });
                if(mayStopSymbol == LexiconSymbol.SkipMaterial)
                {
                    CurrentSymbol = mayStopSymbol;
                }

                if(CurrentSymbol != LexiconSymbol.NA)
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            TokenStreamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        }

        public void Dispose()
        {
            TokenStreamReader.Dispose();
            TokenStreamReader = null;
            LanguageTokens = null;
            CurrentTokenBuffer?.Clear();
        }
    }
}
