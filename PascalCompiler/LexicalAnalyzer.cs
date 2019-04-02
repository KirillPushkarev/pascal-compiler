using System;
using System.Collections.Generic;
using System.Globalization;
using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    public class LexicalAnalyzer
    {
        private IOModule ioModule;

        private char? currentCharacter = ' ';
        private Queue<SymbolEnum> symbolQueue = new Queue<SymbolEnum>();

        public SymbolEnum CurrentSymbol { get; set; }
        public int CurrentLineNumber { get; set; }
        public int CurrentPositionInLine { get; set; }
        public Error Error { get; set; }
        public bool IsFinished { get; set; }

        public HashSet<string> NameTable { get; set; } = new HashSet<string>();
        public string StringConstant { get; set; }
        public int IntConstant { get; set; }
        public double FloatConstant { get; set; }

        private const int MAX_INT_CONSTANT = 32767;
        private const double MAX_FLOAT_CONSTANT = 1.7 * 10E+38;
        private const int MAX_STRING_CONSTANT_LENGTH = 32;

        public LexicalAnalyzer(IOModule ioModule)
        {
            this.ioModule = ioModule;
        }

        public void NextSymbol()
        {
            if (symbolQueue.Count > 0)
            {
                CurrentSymbol = symbolQueue.Dequeue();
                return;
            }

            while (currentCharacter != null && (currentCharacter == ' ' || currentCharacter == '\n'))
            {
                currentCharacter = ioModule.NextCh();
            }

            if (currentCharacter == null)
            {
                IsFinished = true;
                return;
            }

            CurrentLineNumber = ioModule.CurrentRow;
            CurrentPositionInLine = ioModule.CurrentPosition;
            Error = null;

            ScanSymbol();

            if (Error != null)
                SkipToNextSymbol();

            if (currentCharacter == null)
            {
                IsFinished = true;
                return;
            }
        }

        private void ScanSymbol()
        {
            if (LexicalUtils.IsDigit(currentCharacter.Value))
                ScanNumber();
            else if (LexicalUtils.IsStringConstantStart(currentCharacter.Value))
                ScanString();
            else if (LexicalUtils.IsIdentifierStart(currentCharacter.Value))
                ScanIdentifier();
            else
            {
                switch (currentCharacter.Value)
                {
                    case '<':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = SymbolEnum.LessEquals;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                        {
                            if (currentCharacter.Value == '>')
                            {
                                CurrentSymbol = SymbolEnum.NotEquals;
                                currentCharacter = ioModule.NextCh();
                            }
                            else
                                CurrentSymbol = SymbolEnum.Less;
                        }
                        break;
                    case '>':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = SymbolEnum.GreaterEquals;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                        {
                              CurrentSymbol = SymbolEnum.Less;
                        }
                        break;
                    case ':':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '=')
                        {
                            CurrentSymbol = SymbolEnum.Assign;
                            currentCharacter = ioModule.NextCh();
                        }
                        else
                        {
                            CurrentSymbol = SymbolEnum.Colon;
                        }
                        break;
                    case '+':
                        CurrentSymbol = SymbolEnum.Plus;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '-':
                        CurrentSymbol = SymbolEnum.Minus;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '*':
                        CurrentSymbol = SymbolEnum.Star;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '\\':
                        CurrentSymbol = SymbolEnum.Slash;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '=':
                        CurrentSymbol = SymbolEnum.Equals;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '(':
                        currentCharacter = ioModule.NextCh();
                        if (currentCharacter.Value == '*')
                        {
                            ScanMultilineComment();
                        }
                        else
                        {
                            CurrentSymbol = SymbolEnum.LeftRoundBracket;
                        }
                        break;
                    case ')':
                        CurrentSymbol = SymbolEnum.RightRoundBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '[':
                        CurrentSymbol = SymbolEnum.LeftSquareBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ']':
                        CurrentSymbol = SymbolEnum.RightSquareBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '{':
                        CurrentSymbol = SymbolEnum.LeftCurlyBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '}':
                        CurrentSymbol = SymbolEnum.RightCurlyBracket;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '.':
                        CurrentSymbol = SymbolEnum.Dot;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ',':
                        CurrentSymbol = SymbolEnum.Comma;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case '^':
                        CurrentSymbol = SymbolEnum.Arrow;
                        currentCharacter = ioModule.NextCh();
                        break;
                    case ';':
                        CurrentSymbol = SymbolEnum.Semicolon;
                        currentCharacter = ioModule.NextCh();
                        break;
                    default:
                        Error = ioModule.AddError(6, CurrentLineNumber, CurrentPositionInLine);
                        break;
                }
            }
        }

        private void SkipToNextSymbol()
        {
            while (currentCharacter != null && currentCharacter.Value != ' ' && currentCharacter.Value != '\n')
            {
                currentCharacter = ioModule.NextCh();
            }
        }

        private void ScanNumber()
        {
            string scannedSymbol = "";
            bool hasDecimalSeparator = false;
            char prevCh = currentCharacter.Value;

            while (LexicalUtils.IsDigit(currentCharacter.Value) || LexicalUtils.IsDecimalSeparator(currentCharacter.Value))
            {
                if (LexicalUtils.IsDecimalSeparator(currentCharacter.Value))
                {
                    if (hasDecimalSeparator)
                    {
                        if (LexicalUtils.IsDecimalSeparator(prevCh))
                        {
                            CurrentSymbol = SymbolEnum.IntConstant;
                            IntConstant = int.Parse(scannedSymbol.Substring(0, scannedSymbol.Length - 1));
                            symbolQueue.Enqueue(SymbolEnum.TwoDots);
                            currentCharacter = ioModule.NextCh();
                            return;
                        }
                        else
                        {
                            break;
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
                double result;
                if (double.TryParse(scannedSymbol, NumberStyles.Any, CultureInfo.InvariantCulture, out result) && Math.Abs(result) < MAX_FLOAT_CONSTANT)
                {
                    CurrentSymbol = SymbolEnum.FLoatConstant;
                    FloatConstant = result;
                }
                else
                {
                    Error = ioModule.AddError(207, CurrentLineNumber, CurrentPositionInLine);
                }
            }
            else
            {
                int result;
                if (int.TryParse(scannedSymbol, out result) && Math.Abs(result) < MAX_INT_CONSTANT)
                {
                    CurrentSymbol = SymbolEnum.IntConstant;
                    IntConstant = result;
                }
                else
                {
                    Error = ioModule.AddError(203, CurrentLineNumber, CurrentPositionInLine);
                }
            }
        }

        private void ScanString()
        {
            string scannedSymbol = "";
            do
            {
                currentCharacter = ioModule.NextCh();
                if (currentCharacter != '\'' && scannedSymbol.Length < MAX_STRING_CONSTANT_LENGTH)
                {
                    scannedSymbol += currentCharacter.Value.ToString();
                }
            }
            while (currentCharacter != null && currentCharacter != '\'' && currentCharacter != '\n');

            if (scannedSymbol.Length >= MAX_STRING_CONSTANT_LENGTH)
            {
                Error = ioModule.AddError(75, CurrentLineNumber, CurrentPositionInLine);
                return;
            }

            if (currentCharacter != null && currentCharacter != '\n' && scannedSymbol.Length != 0)
            {
                CurrentSymbol = SymbolEnum.CharConstant;
                StringConstant = scannedSymbol;
                currentCharacter = ioModule.NextCh();
            }
            else
            {
                Error = ioModule.AddError(75, CurrentLineNumber, CurrentPositionInLine);
            }
        }

        private void ScanIdentifier()
        {
            string scannedSymbol = "";
            while (LexicalUtils.IsIdentifierChar(currentCharacter.Value))
            {
                scannedSymbol += currentCharacter.Value.ToString();
                currentCharacter = ioModule.NextCh();
            }

            SymbolEnum keyword;
            if (Symbol.keywordMapping.TryGetValue(scannedSymbol, out keyword))
            {
                CurrentSymbol = keyword;
            }
            else
            {
                CurrentSymbol = SymbolEnum.Identifier;
                NameTable.Add(scannedSymbol);
            }
        }

        private void ScanMultilineComment()
        {
            char prevCharacter = '(';
            do
            {
                prevCharacter = currentCharacter.Value;
                currentCharacter = ioModule.NextCh();
            }
            while (currentCharacter != null && prevCharacter.ToString() + currentCharacter.ToString() != "*)");

            CurrentLineNumber = ioModule.CurrentRow;
            CurrentPositionInLine = ioModule.CurrentPosition;

            if (currentCharacter != null)
            {
                currentCharacter = ioModule.NextCh();
                NextSymbol();
            }
            else
            {
                Error = ioModule.AddError(86, CurrentLineNumber, CurrentPositionInLine);
            }
        }
    }
}
