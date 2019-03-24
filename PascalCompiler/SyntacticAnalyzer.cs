using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Accept(SymbolEnum.Identifier);
            while (lexicalAnalyzer.CurrentSymbol == SymbolEnum.Comma)
            {
                Accept(SymbolEnum.Comma);
                Accept(SymbolEnum.Identifier);
            }
            Accept(SymbolEnum.Colon);
            Type();
        }

        private void VarDeclaration()
        {

        }

        private void Type()
        {

        }

        private void ProcFuncPart()
        {
        }

        private void StatementPart()
        {
            Accept(SymbolEnum.BeginSy);
            Accept(SymbolEnum.EndSy);
        }
    }
}
