using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PascalCompiler
{
    class LexicalAnalyzer
    {
        IOModule ioModule;
        char? currentCharacter = ' ';
        Dictionary<string, Symbol> keywordMapping = new Dictionary<string, Symbol>()
        {
            { "do", Symbol.DoSy },
            { "if", Symbol.IfSy },
            { "in", Symbol.InSy },
            { "of", Symbol.OfSy },
            { "type", Symbol.TypeSy },
        };

        public Symbol CurrentSymbol { get; set; }
        public HashSet<string> NameTable { get; set; } = new HashSet<string>();
        public string StringConstant { get; set; }
        public int IntConstant { get; set; }
        public double FloatConstant { get; set; }

        public LexicalAnalyzer(IOModule ioModule)
        {
            this.ioModule = ioModule;
        }

        public void NextSymbol()
        {
            while (currentCharacter.Value == ' ')
            {
                currentCharacter = ioModule.NextCh();
            }

            if (currentCharacter == null)
                return;
            ScanSymbol();
        }

        private void ScanSymbol()
        {
            if (IsDigit(currentCharacter.Value))
                ScanNumber();
            else if (currentCharacter.Value == '\'')
                ScanString();
            else if (IsIdentifierStart(currentCharacter.Value))
                ScanIdentifier();
            else
            {
                switch (currentCharacter.Value)
                {
                    case '<':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = Symbol.LessEquals;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                        {
                            if (currentCharacter.Value == '>')
                            {
                                CurrentSymbol = Symbol.NotEquals;
                                currentCharacter = ioModule.NextCh();
                            }
                            else
                                CurrentSymbol = Symbol.Less;
                        }
                        break;
                    case '>':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = Symbol.GreaterEquals;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                        {
                              CurrentSymbol = Symbol.Less;
                        }
                        break;
                    case ':':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = Symbol.Assign;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                            CurrentSymbol = Symbol.Colon;
                        break;
                    case ';':
                        CurrentSymbol = Symbol.Semicolon;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ',':
                        CurrentSymbol = Symbol.Comma;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '(':
                        CurrentSymbol = Symbol.LeftRoundBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ')':
                        CurrentSymbol = Symbol.RightRoundBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '[':
                        CurrentSymbol = Symbol.LeftSquareBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ']':
                        CurrentSymbol = Symbol.RightSquareBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '{':
                        CurrentSymbol = Symbol.LeftCurlyBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '}':
                        CurrentSymbol = Symbol.RightCurlyBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '*':
                        CurrentSymbol = Symbol.Star;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '\\':
                        CurrentSymbol = Symbol.Slash;
                        currentCharacter = ioModule.NextCh();
                        break;
                }
            }
        }

        private bool IsDigit(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[0-9]+$");
        }

        private bool IsDecimalSeparator(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[\.]+$");
        }

        private bool IsIdentifierStart(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[a-z_]+$");
        }

        private bool IsIdentifierChar(char ch)
        {
            return Regex.IsMatch(ch.ToString(), @"^[a-z0-9_]+$");
        }

        private void ScanNumber()
        {
            string scannedSymbol = "";
            bool hasDecimalSeparator = false;
            while (IsDigit(currentCharacter.Value) || IsDecimalSeparator(currentCharacter.Value))
            {
                scannedSymbol += currentCharacter.Value.ToString();
                if (IsDecimalSeparator(currentCharacter.Value))
                    hasDecimalSeparator = true;
                currentCharacter = ioModule.NextCh();
            }

            if (hasDecimalSeparator)
            {
                CurrentSymbol = Symbol.FLoatConstant;
                FloatConstant = double.Parse(scannedSymbol);
            }
            else
            {
                CurrentSymbol = Symbol.IntConstant;
                IntConstant = int.Parse(scannedSymbol);
            }
        }

        private void ScanString()
        {
            string scannedSymbol = "";
            while (currentCharacter.Value != '\'')
            {
                scannedSymbol += currentCharacter.Value.ToString();
                currentCharacter = ioModule.NextCh();
            }

            CurrentSymbol = Symbol.CharConstant;
            StringConstant = scannedSymbol;
        }

        private void ScanIdentifier()
        {
            string scannedSymbol = "";
            while (IsIdentifierChar(currentCharacter.Value))
            {
                scannedSymbol += currentCharacter.Value.ToString();
                currentCharacter = ioModule.NextCh();
            }

            Symbol keyword;
            if (keywordMapping.TryGetValue(scannedSymbol, out keyword))
            {
                CurrentSymbol = keyword;
            }
            else
            {
                CurrentSymbol = Symbol.Identifier;
                NameTable.Add(scannedSymbol);
            }
        }
    }
}
