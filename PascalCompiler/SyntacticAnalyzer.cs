using static PascalCompiler.Symbol;

namespace PascalCompiler
{
    public class SyntacticAnalyzer
    {
        private IOModule ioModule;
        private LexicalAnalyzer lexicalAnalyzer;

        public SyntacticAnalyzer(IOModule ioModule, LexicalAnalyzer lexicalAnalyzer)
        {
            this.ioModule = ioModule;
            this.lexicalAnalyzer = lexicalAnalyzer;
        }

        public void Run()
        {
            Program();
        }

        private void Accept(SymbolEnum expectedSymbol)
        {
            if (lexicalAnalyzer.CurrentSymbol == expectedSymbol)
            {
                lexicalAnalyzer.NextSymbol();
            }
            else
            {
                ioModule.AddError(lexicalAnalyzer.CurrentLineNumber, lexicalAnalyzer.CurrentPositionInLine, expectedSymbol);
            }
        }

        private void Program()
        {
            ProgramHead();
            Accept(SymbolEnum.Semicolon);
            Block();
            Accept(SymbolEnum.Dot);
        }

        private void ProgramHead()
        {
            Accept(SymbolEnum.ProgramSy);
            Accept(SymbolEnum.Identifier);
        }

        private void Block()
        {
            LabelPart();
            ConstPart();
            TypePart();
            VarPart();
            ProcFuncPart();
            StatementPart();
        }

        private void LabelPart()
        {
        }

        private void ConstPart()
        {
        }

        private void TypePart()
        {
        }

        private void VarPart()
        {
            Accept(SymbolEnum.VarSy);
            while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Identifier)
            {
                VarDeclaration();
                Accept(SymbolEnum.Semicolon);
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

        private void Type()
        {
            SimpleType();
        }

        private void SimpleType()
        {
            switch (lexicalAnalyzer.CurrentSymbol)
            {
                case SymbolEnum.BooleanSy:
                case SymbolEnum.ByteSy:
                case SymbolEnum.CharSy:
                case SymbolEnum.IntegerSy:
                case SymbolEnum.RealSy:
                case SymbolEnum.StringSy:
                    Accept(lexicalAnalyzer.CurrentSymbol);
                    break;
                case SymbolEnum.IntConstant:
                case SymbolEnum.CharConstant:
                    SubrangeType(lexicalAnalyzer.CurrentSymbol);
                    break;
            }
        }

        private void SubrangeType(SymbolEnum symbol)
        {
            Accept(symbol);
            Accept(SymbolEnum.TwoDots);
            Accept(symbol);
        }

        private void TypeIdentifier()
        {

        }

        private void ProcFuncPart()
        {
        }

        private void StatementPart()
        {
            CompoundStatement();
        }

        private void CompoundStatement()
        {

        }

        private void SimpleStatement()
        {

        }

        private void AssignmentStatement()
        {
            Variable();
            Accept(SymbolEnum.Assign);
        }

        private void Variable()
        {
            Accept(SymbolEnum.Identifier);
        }

        private void Expression()
        {
            SimpleExpression();
        }

        private void SimpleExpression()
        {

        }

        private void Term()
        {

        }

        private void Factor()
        {

        }
    }
}
