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
            { "program", Symbol.ProgramSy },
            { "var", Symbol.VarSy },
            { "procedure", Symbol.ProcedureSy },
            { "array", Symbol.ArraySy },
            { "integer", Symbol.IntegerSy },
            { "real", Symbol.RealSy },
        };

        private Symbol CurrentSymbol { get; set; }
        private Queue<Symbol> SymbolQueue { get; set; } = new Queue<Symbol>();
        private int currentLineNumber;
        private int currentPositionInLine;

        public HashSet<string> NameTable { get; set; } = new HashSet<string>();
        public string StringConstant { get; set; }
        public int IntConstant { get; set; }
        public double FloatConstant { get; set; }
        public Error Error { get; set; }

        public LexicalAnalyzer(IOModule ioModule)
        {
            this.ioModule = ioModule;
        }

        public void NextSymbol()
        {
            if (SymbolQueue.Count > 0)
            {
                CurrentSymbol = SymbolQueue.Dequeue();
                return;
            }

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
                    case '+':
                        CurrentSymbol = Symbol.Plus;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '-':
                        CurrentSymbol = Symbol.Minus;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '*':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == ')')
                        {
                            CurrentSymbol = Symbol.RightComment;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                            CurrentSymbol = Symbol.Star;
                        break;
                    case '\\':
                        CurrentSymbol = Symbol.Slash;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '=':
                        CurrentSymbol = Symbol.Equals;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '(':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '*')
                        {
                            CurrentSymbol = Symbol.LeftComment;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                            CurrentSymbol = Symbol.LeftRoundBracket;
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
                    case '.':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '.')
                        {
                            CurrentSymbol = Symbol.TwoPoints;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                            CurrentSymbol = Symbol.Point;
                        break;
                    case ',':
                        CurrentSymbol = Symbol.Comma;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '^':
                        CurrentSymbol = Symbol.Arrow;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ';':
                        CurrentSymbol = Symbol.Semicolon;
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
            char prevCh = currentCharacter.Value;

            while (IsDigit(currentCharacter.Value) || IsDecimalSeparator(currentCharacter.Value))
            {
                if (IsDecimalSeparator(currentCharacter.Value))
                {
                    prevCh = currentCharacter.Value;
                    currentCharacter = ioModule.NextCh();
                    if (IsDecimalSeparator(currentCharacter.Value))
                    {
                        if (hasDecimalSeparator)
                        {
                            if (prevCh == '.')
                            {
                                CurrentSymbol = Symbol.IntConstant;
                                IntConstant = int.Parse(scannedSymbol);
                                SymbolQueue.Enqueue(Symbol.TwoPoints);
                                ioModule.NextCh();
                                return;
                            }
                            else
                            {
                                Error = new Error();
                                ioModule.AddError();
                                return;
                            }
                        }
                    }

                    hasDecimalSeparator = true;
                }

                scannedSymbol += currentCharacter.Value.ToString();
                prevCh = currentCharacter.Value;
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
