using System;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    public class SyntacticAnalyzer
    {
        private IOModule ioModule;
        private LexicalAnalyzer lexicalAnalyzer;

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
            if (lexicalAnalyzer.Error != null)
            {
                throw new Exception("Ошибка в лексическом анализаторе.");
            }

            if (lexicalAnalyzer.CurrentSymbol != expectedSymbol)
            {
                ioModule.AddError(
                    lexicalAnalyzer.CurrentLineNumber, 
                    lexicalAnalyzer.CurrentPositionInLine, 
                    expectedSymbol
                    );

                throw new Exception("Ошибка в синтаксическом анализаторе.");
            }

            lexicalAnalyzer.NextSymbol();
        }

        private void NextSymbol()
        {
            if (lexicalAnalyzer.Error != null)
            {
                throw new Exception("Ошибка в лексическом анализаторе.");
            }

            lexicalAnalyzer.NextSymbol();
        }

        public void Run()
        {
            NextSymbol();
            Program();
        }

        private void Program()
        {
            ProgramHeading();
            Accept(SymbolEnum.Semicolon);
            Block();
            Accept(SymbolEnum.Dot);
        }

        private void ProgramHeading()
        {
            Accept(SymbolEnum.ProgramSy);
            Accept(SymbolEnum.Identifier);
        }

        private void Block()
        {
            LabelDeclarationPart();
            ConstantDefinitionPart();
            TypeDefinitionPart();
            VarDeclarationPart();
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
            if (lexicalAnalyzer.CurrentSymbol == SymbolEnum.TypeSy)
            {
                Accept(SymbolEnum.TypeSy);
                while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Identifier)
                {
                    Accept(SymbolEnum.Identifier);
                    Accept(SymbolEnum.Equals);
                    Type();
                    Accept(SymbolEnum.Semicolon);
                }
            }
        }

        private void Type()
        {
            if (IsStartSimpleType(CurrentSymbol))
                SimpleType();
            else if (IsStartStructuredType(CurrentSymbol))
                StructuredType();
            else if (IsStartPointerType(CurrentSymbol))
                PointerType();

            // Add error
        }

        private bool IsStartSimpleType(SymbolEnum symbol)
        {
            return IsStartScalarType(symbol) || IsStartSubrangeType(symbol) || IsStartEnumerableType(symbol);
        }

        private bool IsStartScalarType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.BooleanSy, SymbolEnum.CharSy, SymbolEnum.IntegerSy, SymbolEnum.RealSy, SymbolEnum.StringSy };
            return allowedSymbols.Contains(symbol);
        }

        private bool IsStartSubrangeType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.IntConstant, SymbolEnum.CharConstant };
            return allowedSymbols.Contains(symbol);
        }

        private bool IsStartEnumerableType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.LeftRoundBracket };
            return allowedSymbols.Contains(symbol);
        }

        private void SimpleType()
        {
            if (IsStartScalarType(CurrentSymbol))
                NextSymbol();
            else if (IsStartSubrangeType(CurrentSymbol))
                SubrangeType(CurrentSymbol);
            else if (IsStartEnumerableType(CurrentSymbol))
                EnumerableType();

            // Add error
        }

        private void SubrangeType(SymbolEnum symbol)
        {
            Accept(symbol);
            Accept(SymbolEnum.TwoDots);
            Accept(symbol);
        }

        private void EnumerableType()
        {
            // TODO
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

            // Add error
        }

        private void ArrayType()
        {
            Accept(SymbolEnum.ArraySy);
            Accept(SymbolEnum.LeftSquareBracket);
            IndexType();
            while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Comma)
            {
                Accept(SymbolEnum.Comma);
                IndexType();
            }
            Accept(SymbolEnum.RightSquareBracket);
            Accept(SymbolEnum.OfSy);
            Type();
        }

        private void IndexType()
        {
            switch (lexicalAnalyzer.CurrentSymbol)
            {
                case SymbolEnum.BooleanSy:
                case SymbolEnum.ByteSy:
                case SymbolEnum.CharSy:
                case SymbolEnum.IntegerSy:
                    NextSymbol();
                    break;
                case SymbolEnum.IntConstant:
                case SymbolEnum.CharConstant:
                    SubrangeType(lexicalAnalyzer.CurrentSymbol);
                    break;
            }

            // Add error
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

        private void VarDeclarationPart()
        {
            if (lexicalAnalyzer.CurrentSymbol == SymbolEnum.VarSy)
            {
                Accept(SymbolEnum.VarSy);
                do
                {
                    VarDeclaration();
                    Accept(SymbolEnum.Semicolon);
                }
                while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Identifier);
            }
        }

        private void VarDeclaration()
        {
            Accept(SymbolEnum.Identifier);
            while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Comma)
            {
                Accept(SymbolEnum.Comma);
                Accept(SymbolEnum.Identifier);
            }
            Accept(SymbolEnum.Colon);
            Type();
        }

        private void ProcAndFuncDeclarationPart()
        {

        }

        private void StatementPart()
        {
            CompoundStatement();
        }

        private void CompoundStatement()
        {
            Accept(SymbolEnum.BeginSy);
            Statement();
            while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Semicolon)
            {
                Accept(SymbolEnum.Semicolon);
                Statement();
            }
            Accept(SymbolEnum.EndSy);
        }

        private void Statement()
        {
            if (IsStartStructuredStatement(CurrentSymbol))
                StructuredStatement();
            else 
                SimpleStatement();
        }

        private void SimpleStatement()
        {
            AssignmentStatement();
        }

        private bool IsStartStructuredStatement(SymbolEnum symbol)
        {
            return CurrentSymbol == SymbolEnum.BeginSy || CurrentSymbol == SymbolEnum.IfSy || IsStartRepetitiveStatement(symbol) || CurrentSymbol == SymbolEnum.WithSy;
        }

        private void StructuredStatement()
        {
            if (CurrentSymbol == SymbolEnum.BeginSy)
                CompoundStatement();
            else if (CurrentSymbol == SymbolEnum.IfSy)
                ConditionalStatement();
            else if (IsStartRepetitiveStatement(CurrentSymbol))
                RepetitiveStatement();
            else if (CurrentSymbol == SymbolEnum.WithSy)
                WithStatement();
        }

        private void AssignmentStatement()
        {
            Accept(SymbolEnum.Identifier);
            Accept(SymbolEnum.Assign);
            Expression();
        }

        private void ProcedureStatement()
        {
            Accept(SymbolEnum.Identifier);
            if (CurrentSymbol == SymbolEnum.LeftRoundBracket)
            {
                Accept(SymbolEnum.LeftRoundBracket);
                Accept(SymbolEnum.Identifier);
                Accept(SymbolEnum.RightRoundBracket);
            }
        }

        private void Expression()
        {
            SimpleExpression();

            var currentSymbol = lexicalAnalyzer.CurrentSymbol;
            if (currentSymbol == SymbolEnum.Equals ||
                currentSymbol == SymbolEnum.NotEquals ||
                currentSymbol == SymbolEnum.Less ||
                currentSymbol == SymbolEnum.LessEquals ||
                currentSymbol == SymbolEnum.GreaterEquals ||
                currentSymbol == SymbolEnum.Greater ||
                currentSymbol == SymbolEnum.InSy)
            {
                RelationalOperator();
                SimpleExpression();
            }
        }

        private void SimpleExpression()
        {
            Term();

            var currentSymbol = lexicalAnalyzer.CurrentSymbol;
            if (currentSymbol == SymbolEnum.Plus ||
                currentSymbol == SymbolEnum.Minus ||
                currentSymbol == SymbolEnum.OrSy)
            {
                AddingOperator();
                Term();
            }
        }

        private void Term()
        {
            Factor();

            var currentSymbol = lexicalAnalyzer.CurrentSymbol;
            if (currentSymbol == SymbolEnum.Star ||
                currentSymbol == SymbolEnum.Slash ||
                currentSymbol == SymbolEnum.DivSy ||
                currentSymbol == SymbolEnum.ModSy ||
                currentSymbol == SymbolEnum.AndSy)
            {
                MultiplyingOperator();
                Factor();
            }
        }

        private void Factor()
        {
            if (CurrentSymbol == SymbolEnum.NotSy)
            {
                Accept(SymbolEnum.NotSy);
            }

            Expression();
        }

        private void RelationalOperator()
        {
            Accept(lexicalAnalyzer.CurrentSymbol);
        }

        private void MultiplyingOperator()
        {
            Accept(lexicalAnalyzer.CurrentSymbol);
        }

        private void AddingOperator()
        {
            Accept(lexicalAnalyzer.CurrentSymbol);
        }

        private void ConditionalStatement()
        {
            if (lexicalAnalyzer.CurrentSymbol == SymbolEnum.IfSy)
            {
                IfStatement();
            }
            else if (lexicalAnalyzer.CurrentSymbol == SymbolEnum.CaseSy)
            {
                CaseStatement();
            }
        }

        private bool IsStartRepetitiveStatement(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.WhileSy, SymbolEnum.RepeatSy, SymbolEnum.ForSy };
            return allowedSymbols.Contains(symbol);
        }

        private void RepetitiveStatement()
        {
            switch (lexicalAnalyzer.CurrentSymbol)
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

        private void WithStatement()
        {
            // TODO
        }

        private void IfStatement()
        {
            Accept(SymbolEnum.IfSy);
            Expression();
            Accept(SymbolEnum.ThenSy);
            Statement();
            if (lexicalAnalyzer.CurrentSymbol == SymbolEnum.ElseSy)
            {
                Accept(SymbolEnum.ElseSy);
                Statement();
            }
        }

        private void CaseStatement()
        {
            // TODO
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
            Accept(SymbolEnum.UntilSy);
            Expression();
        }

        private void ForStatement()
        {
            Accept(SymbolEnum.ForSy);
            ControlVariable();
            Accept(SymbolEnum.Assign);
            ForList();
            Accept(SymbolEnum.DoSy);
            Statement();
        }

        private void ControlVariable()
        {
            Accept(SymbolEnum.Identifier);
        }

        private void ForList()
        {
            Expression();

            switch (lexicalAnalyzer.CurrentSymbol)
            {
                case SymbolEnum.ToSy:
                    Accept(SymbolEnum.ToSy);
                    break;
                case SymbolEnum.DowntoSy:
                    Accept(SymbolEnum.DowntoSy);
                    break;
            }

            Expression();
        }
    }
}
