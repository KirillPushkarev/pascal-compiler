using System.Collections.Generic;
using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    class LexicalAnalyzer
    {
        private IOModule ioModule;

        private char? currentCharacter = ' ';
        private int currentLineNumber;
        private int currentPositionInLine;
        private Queue<SymbolEnum> symbolQueue = new Queue<SymbolEnum>();

        public SymbolEnum CurrentSymbol { get; set; }
        public bool IsFinished { get; set; }

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
            if (symbolQueue.Count > 0)
            {
                CurrentSymbol = symbolQueue.Dequeue();
                return;
            }

            while (currentCharacter != null && (currentCharacter == ' ' || currentCharacter == '\n'))
            {
                if (currentCharacter == '\n')
                {
                    currentLineNumber = ioModule.CurrentRow;
                }
                currentCharacter = ioModule.NextCh();
            }

            if (currentCharacter == null)
            {
                IsFinished = true;
                return;
            }

            currentLineNumber = ioModule.CurrentRow;
            currentPositionInLine = ioModule.CurrentPosition;
            Error = null;

            ScanSymbol();

            if (Error != null)
                SkipToNextSymbol();
        }

        private void ScanSymbol()
        {
            if (LexicalUtils.IsDigit(currentCharacter.Value))
                ScanNumber();
            else if (currentCharacter.Value == '\'')
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
                        CurrentSymbol = SymbolEnum.Point;
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
                        Error = ioModule.AddError(6, currentLineNumber, currentPositionInLine);
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
                        if (prevCh == '.')
                        {
                            CurrentSymbol = SymbolEnum.IntConstant;
                            IntConstant = int.Parse(scannedSymbol.Substring(0, scannedSymbol.Length - 1));
                            symbolQueue.Enqueue(SymbolEnum.TwoPoints);
                            currentCharacter = ioModule.NextCh();
                            return;
                        }
                        else
                        {
                            return;
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
                CurrentSymbol = SymbolEnum.FLoatConstant;
                FloatConstant = double.Parse(scannedSymbol);
            }
            else
            {
                CurrentSymbol = SymbolEnum.IntConstant;
                IntConstant = int.Parse(scannedSymbol);
            }
        }

        private void ScanString()
        {
            string scannedSymbol = "";
            do
            {
                currentCharacter = ioModule.NextCh();
                if (currentCharacter != '\'')
                {
                    scannedSymbol += currentCharacter.Value.ToString();
                }
            }
            while (currentCharacter != null && currentCharacter != '\'' && currentCharacter != '\n');

            if (currentCharacter != null && currentCharacter != '\n' && scannedSymbol.Length != 0)
            {
                CurrentSymbol = SymbolEnum.CharConstant;
                StringConstant = scannedSymbol;
                currentCharacter = ioModule.NextCh();
            }
            else
            {
                Error = ioModule.AddError(75, currentLineNumber, currentPositionInLine);
                if (currentCharacter == '\n')
                {
                    currentLineNumber = ioModule.CurrentRow;
                }
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
                if (prevCharacter == '\n')
                {
                    currentLineNumber = ioModule.CurrentRow;
                }
                currentCharacter = ioModule.NextCh();
            }
            while (currentCharacter != null && prevCharacter.ToString() + currentCharacter.ToString() != "*)");

            if (currentCharacter != null)
            {
                currentCharacter = ioModule.NextCh();
                NextSymbol();
            }
            else
            {
                Error = ioModule.AddError(86, currentLineNumber, currentPositionInLine);
            }
        }
    }
}
