using PascalCompiler.SyntaticAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    public class SyntacticAnalyzer
    {
        private IOModule ioModule;
        private LexicalAnalyzer lexicalAnalyzer;

        private const int FORBIDDEN_SYMBOL_ERROR_CODE = 6;

        private SymbolEnum CurrentSymbol
        {
            get { return lexicalAnalyzer.CurrentSymbol; }
        }

        public SyntacticAnalyzer(IOModule ioModule, LexicalAnalyzer lexicalAnalyzer)
        {
            this.ioModule = ioModule;
            this.lexicalAnalyzer = lexicalAnalyzer;
        }

        private void Accept(SymbolEnum expectedSymbol)
        {
            if (lexicalAnalyzer.CurrentSymbol != expectedSymbol)
            {
                Error(symbolToErrorCodeMapping[expectedSymbol]);
            }
            else
            {
                NextSymbol();
            }
        }

        private void NextSymbol()
        {
            do
            {
                lexicalAnalyzer.NextSymbol();
            }
            while (lexicalAnalyzer.Error != null && !lexicalAnalyzer.IsFinished);
        }

        private void Error(int errorCode)
        {
            ioModule.AddError(
                errorCode,
                lexicalAnalyzer.CurrentLineNumber,
                lexicalAnalyzer.CurrentPositionInLine
                );
        }

        private void NeutralizerDecorator(
            Action<HashSet<SymbolEnum>> method, 
            HashSet<SymbolEnum> starters,
            HashSet<SymbolEnum> followers,
            int errorCode,
            HashSet<SymbolEnum> parentFollowers = null
            )
        {
            if (parentFollowers != null)
            {
                followers = new HashSet<SymbolEnum>(followers.Concat(parentFollowers));
            }

            if (!starters.Contains(CurrentSymbol))
            {
                Error(errorCode);
                SkipToBefore(starters, followers);
            }
            if (starters.Contains(CurrentSymbol))
            {
                method(followers);
                if (!followers.Contains(CurrentSymbol))
                {
                    Error(FORBIDDEN_SYMBOL_ERROR_CODE);
                    SkipToAfter(followers);
                }
            }
        }

        public void Run()
        {
            NextSymbol();
            Program();
        }

        private void SkipToBefore(HashSet<SymbolEnum> starters, HashSet<SymbolEnum> followers)
        {
            while (!starters.Contains(CurrentSymbol) && !followers.Contains(CurrentSymbol) && !lexicalAnalyzer.IsFinished)
            {
                NextSymbol();
            }
        }

        private void SkipToAfter(HashSet<SymbolEnum> followers)
        {
            while (!followers.Contains(CurrentSymbol) && !lexicalAnalyzer.IsFinished)
            {
                NextSymbol();
            }
        }

        private void Program()
        {
            ProgramHeading();
            Accept(SymbolEnum.Semicolon);
            NeutralizerDecorator(Block, Starters.Block, Followers.Block, 18);
            Accept(SymbolEnum.Dot);
        }

        private void ProgramHeading()
        {
            Accept(SymbolEnum.ProgramSy);
            Accept(SymbolEnum.Identifier);
        }

        private void Block(HashSet<SymbolEnum> followers)
        {
            LabelDeclarationPart();
            ConstantDefinitionPart();
            TypeDefinitionPart();
            NeutralizerDecorator(VarDeclarationPart, Starters.VarDeclarationPart, Followers.VarDeclarationPart, 18, followers);
            ProcAndFuncDeclarationPart();
            StatementPart();
        }

        private void LabelDeclarationPart()
        {
            // TODO
        }

        private void ConstantDefinitionPart()
        {
            // TODO
        }

        private void TypeDefinitionPart()
        {
            if (CurrentSymbol == SymbolEnum.TypeSy)
            {
                NextSymbol();
                while (CurrentSymbol == SymbolEnum.Identifier)
                {
                    Accept(SymbolEnum.Identifier);
                    Accept(SymbolEnum.Equals);
                    Type();
                    Accept(SymbolEnum.Semicolon);
                }
            }
        }

        private void Type(HashSet<SymbolEnum> followers = null)
        {
            if (IsStartSimpleType(CurrentSymbol))
                SimpleType();
            else if (IsStartStructuredType(CurrentSymbol))
                StructuredType();
            else if (IsStartPointerType(CurrentSymbol))
                PointerType();
        }

        private bool IsStartSimpleType(SymbolEnum symbol)
        {
            return IsStartSubrangeType(symbol) || IsStartEnumerableType(symbol) || symbol == SymbolEnum.Identifier;
        }

        private bool IsStartEnumerableType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { };
            return allowedSymbols.Contains(symbol);
        }

        private bool IsStartSubrangeType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.IntConstant, SymbolEnum.CharConstant };
            return allowedSymbols.Contains(symbol);
        }

        private void SimpleType()
        {
            if (IsStartEnumerableType(CurrentSymbol))
                EnumerableType();
            else if (IsStartSubrangeType(CurrentSymbol))
                SubrangeType(CurrentSymbol);
            else if (CurrentSymbol == SymbolEnum.Identifier)
                NextSymbol();
        }

        private void EnumerableType()
        {
            // TODO
        }

        private void SubrangeType(SymbolEnum symbol)
        {
            Accept(symbol);
            Accept(SymbolEnum.TwoDots);
            Accept(symbol);
        }

        private bool IsStartStructuredType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.ArraySy, SymbolEnum.RecordSy, SymbolEnum.SetSy, SymbolEnum.FileSy };
            return allowedSymbols.Contains(symbol);
        }

        private void StructuredType()
        {
            if (CurrentSymbol == SymbolEnum.ArraySy)
                ArrayType();
            else if (CurrentSymbol == SymbolEnum.RecordSy)
                RecordType();
            else if (CurrentSymbol == SymbolEnum.SetSy)
                SetType();
            else if (CurrentSymbol == SymbolEnum.FileSy)
                FileType();
        }

        private void ArrayType()
        {
            Accept(SymbolEnum.ArraySy);
            Accept(SymbolEnum.LeftSquareBracket);
            SimpleType();
            while (CurrentSymbol == SymbolEnum.Comma)
            {
                Accept(SymbolEnum.Comma);
                SimpleType();
            }
            Accept(SymbolEnum.RightSquareBracket);
            Accept(SymbolEnum.OfSy);
            Type();
        }

        private void RecordType()
        {
            // TODO
        }

        private void SetType()
        {
            // TODO
        }

        private void FileType()
        {
            // TODO
        }

        private bool IsStartPointerType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.Arrow };
            return allowedSymbols.Contains(symbol);
        }

        private void PointerType()
        {
            // TODO
        }

        private void VarDeclarationPart(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.VarSy)
            {
                NextSymbol();
                do
                {
                    NeutralizerDecorator(VarDeclaration, Starters.VarDeclaration, Followers.VarDeclaration, 2, followers);
                    Accept(SymbolEnum.Semicolon);
                }
                while (CurrentSymbol == SymbolEnum.Identifier);
            }
        }

        private void VarDeclaration(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.Identifier);
            while (CurrentSymbol == SymbolEnum.Comma)
            {
                NextSymbol();
                Accept(SymbolEnum.Identifier);
            }
            Accept(SymbolEnum.Colon);
            NeutralizerDecorator(Type, Starters.Type, Followers.Type, 10, followers);
        }

        private void ProcAndFuncDeclarationPart()
        {
            // TODO
        }

        private void StatementPart()
        {
            CompoundStatement();
        }

        private void CompoundStatement()
        {
            Accept(SymbolEnum.BeginSy);
            Statement();
            while (CurrentSymbol == SymbolEnum.Semicolon)
            {
                NextSymbol();
                Statement();
            }
            Accept(SymbolEnum.EndSy);
        }

        private void Statement()
        {
            if (IsStartStructuredStatement(CurrentSymbol))
                StructuredStatement();
            else if (IsStartSimpleStatement(CurrentSymbol))
                SimpleStatement();
        }

        private bool IsStartSimpleStatement(SymbolEnum symbol)
        {
            return CurrentSymbol == SymbolEnum.Identifier;
        }

        private void SimpleStatement()
        {
            AssignmentStatement();
        }

        private void AssignmentStatement()
        {
            Accept(SymbolEnum.Identifier);
            Accept(SymbolEnum.Assign);
            Expression();
        }

        private bool IsStartStructuredStatement(SymbolEnum symbol)
        {
            return CurrentSymbol == SymbolEnum.BeginSy || CurrentSymbol == SymbolEnum.IfSy || IsStartRepetitiveStatement(symbol) || CurrentSymbol == SymbolEnum.WithSy;
        }

        private void StructuredStatement()
        {
            if (CurrentSymbol == SymbolEnum.BeginSy)
                CompoundStatement();
            else if (CurrentSymbol == SymbolEnum.IfSy || CurrentSymbol == SymbolEnum.CaseSy)
                ConditionalStatement();
            else if (IsStartRepetitiveStatement(CurrentSymbol))
                RepetitiveStatement();
            else if (CurrentSymbol == SymbolEnum.WithSy)
                WithStatement();
        }

        private void ConditionalStatement()
        {
            if (CurrentSymbol == SymbolEnum.IfSy)
            {
                IfStatement();
            }
            else if (CurrentSymbol == SymbolEnum.CaseSy)
            {
                CaseStatement();
            }
        }

        private void IfStatement()
        {
            Accept(SymbolEnum.IfSy);
            Expression();
            Accept(SymbolEnum.ThenSy);
            Statement();
            if (CurrentSymbol == SymbolEnum.ElseSy)
            {
                Accept(SymbolEnum.ElseSy);
                Statement();
            }
        }

        private void CaseStatement()
        {
            // TODO
        }

        private bool IsStartRepetitiveStatement(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.WhileSy, SymbolEnum.RepeatSy, SymbolEnum.ForSy };
            return allowedSymbols.Contains(symbol);
        }

        private void RepetitiveStatement()
        {
            switch (CurrentSymbol)
            {
                case SymbolEnum.WhileSy:
                    WhileStatement();
                    break;
                case SymbolEnum.RepeatSy:
                    RepeatStatement();
                    break;
                case SymbolEnum.ForSy:
                    ForStatement();
                    break;
            }
        }

        private void WhileStatement()
        {
            Accept(SymbolEnum.WhileSy);
            Expression();
            Accept(SymbolEnum.DoSy);
            Statement();
        }

        private void RepeatStatement()
        {
            Accept(SymbolEnum.RepeatSy);
            Statement();
            while (CurrentSymbol == SymbolEnum.Semicolon)
            {
                NextSymbol();
                Statement();
            }
            Accept(SymbolEnum.UntilSy);
            Expression();
        }

        private void ForStatement()
        {
            Accept(SymbolEnum.ForSy);
            ForStatementParameter();
            Accept(SymbolEnum.Assign);
            ForList();
            Accept(SymbolEnum.DoSy);
            Statement();
        }

        private void ForStatementParameter()
        {
            Accept(SymbolEnum.Identifier);
        }

        private void ForList()
        {
            Expression();
            if (CurrentSymbol == SymbolEnum.ToSy || CurrentSymbol == SymbolEnum.DowntoSy)
            {
                NextSymbol();
            }
            Expression();
        }

        private void WithStatement()
        {
            // TODO
        }

        private void Expression()
        {
            SimpleExpression();

            if (CurrentSymbol == SymbolEnum.Equals ||
                CurrentSymbol == SymbolEnum.NotEquals ||
                CurrentSymbol == SymbolEnum.Less ||
                CurrentSymbol == SymbolEnum.LessEquals ||
                CurrentSymbol == SymbolEnum.GreaterEquals ||
                CurrentSymbol == SymbolEnum.Greater ||
                CurrentSymbol == SymbolEnum.InSy)
            {
                NextSymbol();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            if (CurrentSymbol == SymbolEnum.Minus || CurrentSymbol == SymbolEnum.Plus)
            {
                NextSymbol();
            }

            Term();
            if (CurrentSymbol == SymbolEnum.Plus ||
                CurrentSymbol == SymbolEnum.Minus ||
                CurrentSymbol == SymbolEnum.OrSy)
            {
                NextSymbol();
                Term();
            }
        }

        // Слагаемое
        private void Term()
        {
            Factor();

            if (CurrentSymbol == SymbolEnum.Star ||
                CurrentSymbol == SymbolEnum.Slash ||
                CurrentSymbol == SymbolEnum.DivSy ||
                CurrentSymbol == SymbolEnum.ModSy ||
                CurrentSymbol == SymbolEnum.AndSy)
            {
                NextSymbol();
                Factor();
            }
        }

        // Множитель
        private void Factor()
        {
            if (CurrentSymbol == SymbolEnum.NotSy)
            {
                NextSymbol();
                Factor();
            }
            else
            {
                if (IsStartVariable(CurrentSymbol))
                {
                    Variable();
                }
                else if (IsStartUnsignedConstant(CurrentSymbol))
                {
                    NextSymbol();
                }
                else if (CurrentSymbol == SymbolEnum.LeftRoundBracket)
                {
                    Accept(SymbolEnum.LeftRoundBracket);
                    Expression();
                    Accept(SymbolEnum.RightRoundBracket);
                }
            }
        }

        private bool IsStartVariable(SymbolEnum symbol)
        {
            return IsStartEntireVariable(symbol);
        }

        private void Variable()
        {
            EntireVariable();
        }

        private bool IsStartEntireVariable(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.Identifier };
            return allowedSymbols.Contains(symbol);
        }

        private void EntireVariable()
        {
            Accept(SymbolEnum.Identifier);
        }

        private bool IsStartUnsignedConstant(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.IntConstant, SymbolEnum.RealConstant, SymbolEnum.CharConstant, SymbolEnum.Identifier, SymbolEnum.NilSy };
            return allowedSymbols.Contains(symbol);
        }
    }
}
