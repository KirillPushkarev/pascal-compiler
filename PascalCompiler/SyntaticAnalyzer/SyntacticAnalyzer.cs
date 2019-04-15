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

        private SymbolEnum CurrentSymbol { get; set; }
        private Queue<SymbolEnum> symbolQueue = new Queue<SymbolEnum>();
        private Scope CurrentScope => Scope.CurrentScope;
        private TypeDesc lastVarDeclarationType;

        public SyntacticAnalyzer(IOModule ioModule, LexicalAnalyzer lexicalAnalyzer)
        {
            this.ioModule = ioModule;
            this.lexicalAnalyzer = lexicalAnalyzer;
        }

        private void Accept(SymbolEnum expectedSymbol)
        {
            if (lexicalAnalyzer.CurrentSymbol != expectedSymbol)
                Error(symbolToErrorCodeMapping[expectedSymbol]);
            else
                NextSymbol();
        }

        private void NextSymbol()
        {
            if (symbolQueue.Count > 0)
            {
                CurrentSymbol = symbolQueue.Dequeue();
                return;
            }

            do
                lexicalAnalyzer.NextSymbol();
            while (lexicalAnalyzer.Error != null && !lexicalAnalyzer.IsFinished);

            CurrentSymbol = lexicalAnalyzer.CurrentSymbol;
        }

        private void Error(int errorCode)
        {
            ioModule.AddError(errorCode, lexicalAnalyzer.CurrentLineNumber, lexicalAnalyzer.CurrentPositionInLine);
        }

        // Нейтрализация синтаксической конструкции без возврата значения
        private void NeutralizerDecorator
            (
            Action<HashSet<SymbolEnum>> method,
            HashSet<SymbolEnum> starters,
            HashSet<SymbolEnum> followers,
            int errorCode = FORBIDDEN_SYMBOL_ERROR_CODE,
            HashSet<SymbolEnum> parentFollowers = null
            )
        {
            if (parentFollowers != null)
                followers = new HashSet<SymbolEnum>(followers.Concat(parentFollowers));

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

        // Нейтрализация синтаксической конструкции с возвратом значения
        private TResult NeutralizerDecoratorWithReturn<TResult>
            (
            Func<HashSet<SymbolEnum>, TResult> method,
            HashSet<SymbolEnum> starters,
            HashSet<SymbolEnum> followers,
            int errorCode = FORBIDDEN_SYMBOL_ERROR_CODE,
            HashSet<SymbolEnum> parentFollowers = null
            )
        {
            if (parentFollowers != null)
                followers = new HashSet<SymbolEnum>(followers.Concat(parentFollowers));

            if (!starters.Contains(CurrentSymbol))
            {
                Error(errorCode);
                SkipToBefore(starters, followers);
            }
            if (starters.Contains(CurrentSymbol))
            {
                var result = method(followers);
                if (!followers.Contains(CurrentSymbol))
                {
                    Error(FORBIDDEN_SYMBOL_ERROR_CODE);
                    SkipToAfter(followers);
                }
                return result;
            }
            return default(TResult);
        }

        private void SkipToBefore(HashSet<SymbolEnum> starters, HashSet<SymbolEnum> followers)
        {
            while (!starters.Contains(CurrentSymbol) && !followers.Contains(CurrentSymbol) && !lexicalAnalyzer.IsFinished)
                NextSymbol();
        }

        private void SkipToAfter(HashSet<SymbolEnum> followers)
        {
            while (!followers.Contains(CurrentSymbol) && !lexicalAnalyzer.IsFinished)
                NextSymbol();
        }

        // Создание определяющего вхождения идентификатора
        private IdentifierDesc CreateIdentifier(string identifierName, IdentifierClass identifierClass, TypeDesc identifierType = null)
        {
            if (CurrentScope.FindIdentifierInCurrentScope(identifierName) == null)
                return new IdentifierDesc(identifierName, identifierClass, identifierType);
            else
            {
                // Идентификатор уже определен в текущей области действия, ошибка
                Error(101);
                return null;
            }
        }

        // Поиск прикладного вхождения идентификатора
        private IdentifierDesc FindIdentifier(string identifierName, IdentifierClass identifierClass = IdentifierClass.Unknown)
        {
            var identifier = CurrentScope.FindIdentifier(identifierName);
            if (identifier != null)
                return identifier;
            else
            {
                // Определяющее вхождение не найдено, занести в ТИ неопределенный идентификатор
                Error(104);
                identifier = new IdentifierDesc(identifierName, identifierClass, null);
                CurrentScope.IdentifierTable.Add(identifier);
                return identifier;
            }
        }

        // Запуск синтаксического анализатора
        public void Run()
        {
            Scope.CreateInitialScope();

            NextSymbol();
            NeutralizerDecorator(Program, Starters.Program, Followers.Program);

            Scope.Close();
        }

        // <программа> ::= program <имя> (<имя файла>{, <имя файла>}); <блок>.
        private void Program(HashSet<SymbolEnum> followers)
        {
            Scope.Open();

            NeutralizerDecorator(ProgramHeading, Starters.ProgramHeading, Followers.ProgramHeading, 6, followers);
            Accept(SymbolEnum.Semicolon);
            NeutralizerDecorator(Block, Starters.Block, Followers.Block, 18, followers);
            Accept(SymbolEnum.Dot);

            Scope.Close();
        }

        private void ProgramHeading(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.ProgramSy);
            Accept(SymbolEnum.Identifier);
        }

        // <блок> ::= <раздел меток> <раздел констант><раздел типов><раздел переменных><раздел процедур и функций><раздел операторов>
        private void Block(HashSet<SymbolEnum> followers)
        {
            NeutralizerDecorator(TypeDefinitionPart, Starters.TypeDefinitionPart, Followers.TypeDefinitionPart, FORBIDDEN_SYMBOL_ERROR_CODE, followers);
            NeutralizerDecorator(VarDeclarationPart, Starters.VarDeclarationPart, Followers.VarDeclarationPart, 18, followers);
            NeutralizerDecorator(StatementPart, Starters.StatementPart, Followers.StatementPart, FORBIDDEN_SYMBOL_ERROR_CODE, followers);
        }

        // <раздел типов> ::= <пусто> | type <определение типа>; {<определение типа>;}
        // <определение типа> ::= <имя> = <тип>
        private void TypeDefinitionPart(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.TypeSy)
            {
                NextSymbol();
                while (CurrentSymbol == SymbolEnum.Identifier)
                {
                    Accept(SymbolEnum.Identifier);
                    Accept(SymbolEnum.Equals);
                    NeutralizerDecorator(Type, Starters.Type, Followers.Type, 10, followers);
                    Accept(SymbolEnum.Semicolon);
                }
            }
        }

        // <тип> ::= <простой тип> | <составной тип> | <ссылочный тип>
        private void Type(HashSet<SymbolEnum> followers)
        {
            if (IsStartSimpleType(CurrentSymbol))
                SimpleType(followers);
            else if (IsStartStructuredType(CurrentSymbol))
                StructuredType(followers);
        }

        private bool IsStartSimpleType(SymbolEnum symbol)
        {
            return IsStartSubrangeType(symbol) || symbol == SymbolEnum.Identifier;
        }

        // <простой тип> ::= <перечислимый тип> | <ограниченный тип> | <имя типа>
        private void SimpleType(HashSet<SymbolEnum> followers)
        {
            if (IsStartSubrangeType(CurrentSymbol) && CurrentSymbol != SymbolEnum.Identifier)
                NeutralizerDecorator(SubrangeType, Starters.SubrangeType, Followers.SubrangeType, FORBIDDEN_SYMBOL_ERROR_CODE, followers);
            else if (CurrentSymbol == SymbolEnum.Identifier)
            {
                var identifier = FindIdentifier(lexicalAnalyzer.IdentifierName);

                NextSymbol();
                if (CurrentSymbol == SymbolEnum.TwoDots)
                {
                    CurrentSymbol = SymbolEnum.Identifier;
                    symbolQueue.Enqueue(SymbolEnum.TwoDots);
                    NeutralizerDecorator(SubrangeType, Starters.SubrangeType, Followers.SubrangeType, FORBIDDEN_SYMBOL_ERROR_CODE, followers);
                }
                else
                    lastVarDeclarationType = identifier.type;
            }
        }

        private bool IsStartSubrangeType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.Minus, SymbolEnum.Plus, SymbolEnum.IntConstant, SymbolEnum.RealConstant, SymbolEnum.Identifier, SymbolEnum.CharConstant };
            return allowedSymbols.Contains(symbol);
        }

        // <ограниченный тип> ::= <константа> .. <константа>
        private void SubrangeType(HashSet<SymbolEnum> followers)
        {
            Constant();
            Accept(SymbolEnum.TwoDots);
            Constant();
        }

        // <константа> ::= <число без знака> | <знак><число без знака> | <имя константы> | <знак><имя константы> | <строка>
        private void Constant()
        {
            switch (CurrentSymbol)
            {
                case SymbolEnum.Minus:
                case SymbolEnum.Plus:
                    if (CurrentSymbol == SymbolEnum.IntConstant || CurrentSymbol == SymbolEnum.RealConstant || CurrentSymbol == SymbolEnum.Identifier)
                        NextSymbol();
                    break;
                case SymbolEnum.IntConstant:
                case SymbolEnum.RealConstant:
                case SymbolEnum.Identifier:
                    NextSymbol();
                    break;
                case SymbolEnum.CharConstant:
                    NextSymbol();
                    break;
            }
        }

        private bool IsStartStructuredType(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.ArraySy };
            return allowedSymbols.Contains(symbol);
        }

        // <составной тип> ::= <неупакованный составной тип> | packed <неупакованный составной тип>
        // <неупакованный составной тип> ::= <регулярный тип> | <комбинированный тип> | <множественный тип> | <файловый тип> |
        private void StructuredType(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.ArraySy)
                NeutralizerDecorator(ArrayType, Starters.ArrayType, Followers.ArrayType, 6, followers);
        }

        private void ArrayType(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.ArraySy);
            Accept(SymbolEnum.LeftSquareBracket);
            SimpleType(followers);
            while (CurrentSymbol == SymbolEnum.Comma)
            {
                Accept(SymbolEnum.Comma);
                SimpleType(followers);
            }
            Accept(SymbolEnum.RightSquareBracket);
            Accept(SymbolEnum.OfSy);
            NeutralizerDecorator(Type, Starters.Type, Followers.Type, 10, followers);
        }

        private void VarDeclarationPart(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.VarSy)
            {
                NextSymbol();
                do
                    NeutralizerDecorator(VarDeclaration, Starters.VarDeclaration, Followers.VarDeclaration, 2, followers);
                while (CurrentSymbol == SymbolEnum.Identifier);
            }
        }

        private void VarDeclaration(HashSet<SymbolEnum> followers)
        {
            var identifiers = new List<IdentifierDesc>();

            identifiers.Add(CreateIdentifier(lexicalAnalyzer.IdentifierName, IdentifierClass.Vars));
            Accept(SymbolEnum.Identifier);

            while (CurrentSymbol == SymbolEnum.Comma)
            {
                NextSymbol();
                identifiers.Add(CreateIdentifier(lexicalAnalyzer.IdentifierName, IdentifierClass.Vars));
                Accept(SymbolEnum.Identifier);
            }

            Accept(SymbolEnum.Colon);
            NeutralizerDecorator(Type, Starters.Type, Followers.Type, 10, followers);
            Accept(SymbolEnum.Semicolon);

            foreach (var identifier in identifiers)
            {
                if (identifier != null)
                {
                    identifier.type = lastVarDeclarationType;
                    CurrentScope.IdentifierTable.Add(identifier);
                }
            }
        }

        private void StatementPart(HashSet<SymbolEnum> followers)
        {
            NeutralizerDecorator(CompoundStatement, Starters.CompoundStatement, Followers.CompoundStatement, 6, followers);
        }

        private void CompoundStatement(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.BeginSy);
            while (IsStartSimpleStatement(CurrentSymbol) || IsStartStructuredStatement(CurrentSymbol))
            {
                NeutralizerDecorator(Statement, Starters.Statement, Followers.Statement, 6, followers);
                if (CurrentSymbol == SymbolEnum.Semicolon)
                    NextSymbol();
                else
                    break;
            }
            Accept(SymbolEnum.EndSy);
        }

        private void Statement(HashSet<SymbolEnum> followers)
        {
            if (IsStartStructuredStatement(CurrentSymbol))
                NeutralizerDecorator(StructuredStatement, Starters.StructuredStatement, Followers.StructuredStatement, 6, followers);
            else if (IsStartSimpleStatement(CurrentSymbol))
                NeutralizerDecorator(SimpleStatement, Starters.SimpleStatement, Followers.SimpleStatement, 6, followers);
        }

        private bool IsStartSimpleStatement(SymbolEnum symbol)
        {
            return CurrentSymbol == SymbolEnum.Identifier;
        }

        private void SimpleStatement(HashSet<SymbolEnum> followers)
        {
            NeutralizerDecorator(AssignmentStatement, Starters.AssignmentStatement, Followers.AssignmentStatement, 6, followers);
        }

        private void AssignmentStatement(HashSet<SymbolEnum> followers)
        {
            var leftIdentifier = FindIdentifier(lexicalAnalyzer.IdentifierName, IdentifierClass.Vars);
            Accept(SymbolEnum.Identifier);
            Accept(SymbolEnum.Assign);
            var expressionType = NeutralizerDecoratorWithReturn(Expression, Starters.Expression, Followers.Expression, 6, followers);
            if (!TypeValidator.AreTypesAssignmentCompatible(leftIdentifier.type, expressionType))
                Error(145);
        }

        private bool IsStartStructuredStatement(SymbolEnum symbol)
        {
            return CurrentSymbol == SymbolEnum.BeginSy || CurrentSymbol == SymbolEnum.IfSy || IsStartRepetitiveStatement(symbol);
        }

        private void StructuredStatement(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.BeginSy)
                NeutralizerDecorator(CompoundStatement, Starters.CompoundStatement, Followers.CompoundStatement, 6, followers);
            else if (CurrentSymbol == SymbolEnum.IfSy)
                NeutralizerDecorator(ConditionalStatement, Starters.ConditionalStatement, Followers.ConditionalStatement, 6, followers);
            else if (IsStartRepetitiveStatement(CurrentSymbol))
                NeutralizerDecorator(RepetitiveStatement, Starters.RepetitiveStatement, Followers.RepetitiveStatement, 6, followers);
            //else if (CurrentSymbol == SymbolEnum.WithSy)
            //    WithStatement();
        }

        private void ConditionalStatement(HashSet<SymbolEnum> followers)
        {
            if (CurrentSymbol == SymbolEnum.IfSy)
                IfStatement(followers);
        }

        private void IfStatement(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.IfSy);
            NeutralizerDecoratorWithReturn(Expression, Starters.Expression, Followers.Expression, 6, followers);
            Accept(SymbolEnum.ThenSy);
            NeutralizerDecorator(Statement, Starters.Statement, Followers.Statement, 6, followers);
            if (CurrentSymbol == SymbolEnum.ElseSy)
            {
                Accept(SymbolEnum.ElseSy);
                NeutralizerDecorator(Statement, Starters.Statement, Followers.Statement, 6, followers);
            }
        }

        private bool IsStartRepetitiveStatement(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.WhileSy };
            return allowedSymbols.Contains(symbol);
        }

        private void RepetitiveStatement(HashSet<SymbolEnum> followers)
        {
            switch (CurrentSymbol)
            {
                case SymbolEnum.WhileSy:
                    WhileStatement(followers);
                    break;
                    //case SymbolEnum.RepeatSy:
                    //    RepeatStatement();
                    //    break;
                    //case SymbolEnum.ForSy:
                    //    ForStatement();
                    //    break;
            }
        }

        private void WhileStatement(HashSet<SymbolEnum> followers)
        {
            Accept(SymbolEnum.WhileSy);
            NeutralizerDecoratorWithReturn(Expression, Starters.Expression, Followers.Expression, 6, followers);
            Accept(SymbolEnum.DoSy);
            NeutralizerDecorator(Statement, Starters.Statement, Followers.Statement, 6, followers);
        }

        //private void RepeatStatement()
        //{
        //    Accept(SymbolEnum.RepeatSy);
        //    Statement();
        //    while (CurrentSymbol == SymbolEnum.Semicolon)
        //    {
        //        NextSymbol();
        //        Statement();
        //    }
        //    Accept(SymbolEnum.UntilSy);
        //    Expression();
        //}

        //private void ForStatement()
        //{
        //    Accept(SymbolEnum.ForSy);
        //    ForStatementParameter();
        //    Accept(SymbolEnum.Assign);
        //    ForList();
        //    Accept(SymbolEnum.DoSy);
        //    Statement();
        //}

        //private void ForStatementParameter()
        //{
        //    Accept(SymbolEnum.Identifier);
        //}

        //private void ForList()
        //{
        //    Expression();
        //    if (CurrentSymbol == SymbolEnum.ToSy || CurrentSymbol == SymbolEnum.DowntoSy)
        //    {
        //        NextSymbol();
        //    }
        //    Expression();
        //}

        //private void WithStatement()
        //{
        //    // TODO
        //}

        private bool IsComparisonOperator(SymbolEnum symbol)
        {
            SymbolEnum[] allowedSymbols = { SymbolEnum.Equals, SymbolEnum.NotEquals, SymbolEnum.Less, SymbolEnum.LessEquals, SymbolEnum.GreaterEquals, SymbolEnum.Greater, SymbolEnum.InSy };
            return allowedSymbols.Contains(symbol);
        }

        private TypeDesc Expression(HashSet<SymbolEnum> followers)
        {
            var firstOperandType = NeutralizerDecoratorWithReturn(SimpleExpression, Starters.SimpleExpression, Followers.SimpleExpression, 6, followers);

            if (IsComparisonOperator(CurrentSymbol))
            {
                NextSymbol();
                var secondOperandType = NeutralizerDecoratorWithReturn(SimpleExpression, Starters.SimpleExpression, Followers.SimpleExpression, 6, followers);
                if (!TypeValidator.AreTypesComparisonCompatible(firstOperandType, secondOperandType))
                    Error(186);

                return TypeDesc.booleanType;
            }

            return firstOperandType;
        }

        private TypeDesc SimpleExpression(HashSet<SymbolEnum> followers)
        {
            bool hasPrefixSign = false;
            if (CurrentSymbol == SymbolEnum.Minus || CurrentSymbol == SymbolEnum.Plus)
            {
                hasPrefixSign = true;
                NextSymbol();
            }

            var firstOperandType = NeutralizerDecoratorWithReturn(Term, Starters.Term, Followers.Term, 6, followers);
            if (hasPrefixSign)
                if (!TypeValidator.SupportsSign(firstOperandType))
                    Error(211);

            while (CurrentSymbol == SymbolEnum.Plus ||
                CurrentSymbol == SymbolEnum.Minus ||
                CurrentSymbol == SymbolEnum.OrSy)
            {
                var operation = CurrentSymbol;
                NextSymbol();
                var secondOperandType = NeutralizerDecoratorWithReturn(Term, Starters.Term, Followers.Term, 6, followers);
                var expressionType = TypeValidator.GetTypeAfterAddition(firstOperandType, secondOperandType, operation);
                if (expressionType == null)
                    Error(328);

                return expressionType;
            }

            return firstOperandType;
        }

        // Слагаемое
        private TypeDesc Term(HashSet<SymbolEnum> followers)
        {
            var firstOperandType = NeutralizerDecoratorWithReturn(Factor, Starters.Factor, Followers.Factor, 6, followers);

            while (CurrentSymbol == SymbolEnum.Star ||
                CurrentSymbol == SymbolEnum.Slash ||
                CurrentSymbol == SymbolEnum.DivSy ||
                CurrentSymbol == SymbolEnum.ModSy ||
                CurrentSymbol == SymbolEnum.AndSy)
            {
                var operation = CurrentSymbol;
                NextSymbol();
                var secondOperandType = NeutralizerDecoratorWithReturn(Factor, Starters.Factor, Followers.Factor, 6, followers);
                var expressionType = TypeValidator.GetTypeAfterMultiplication(firstOperandType, secondOperandType, operation);
                if (expressionType == null)
                    Error(328);

                return expressionType;
            }

            return firstOperandType;
        }

        // Множитель
        private TypeDesc Factor(HashSet<SymbolEnum> followers)
        {
            TypeDesc expressionType = null;

            if (CurrentSymbol == SymbolEnum.NotSy)
            {
                NextSymbol();
                expressionType = NeutralizerDecoratorWithReturn(Factor, Starters.Factor, Followers.Factor, 6, followers);
            }
            else if (CurrentSymbol == SymbolEnum.IntConstant)
            {
                expressionType = TypeDesc.integerType;
                NextSymbol();
            }
            else if (CurrentSymbol == SymbolEnum.RealConstant)
            {
                expressionType = TypeDesc.realType;
                NextSymbol();
            }
            else if (CurrentSymbol == SymbolEnum.CharConstant)
            {
                expressionType = TypeDesc.charType;
                NextSymbol();
            }
            else if (CurrentSymbol == SymbolEnum.Identifier)
            {
                var identifier = FindIdentifier(lexicalAnalyzer.IdentifierName, IdentifierClass.Vars);
                switch (identifier.identifierClass)
                {
                    case IdentifierClass.Vars:
                        expressionType = identifier.type;
                        Variable();
                        break;
                    case IdentifierClass.Consts:
                        expressionType = identifier.type;
                        NextSymbol();
                        break;
                    case IdentifierClass.Unknown:
                        expressionType = null;
                        break;
                }
            }
            else if (CurrentSymbol == SymbolEnum.LeftRoundBracket)
            {
                NextSymbol();
                expressionType = NeutralizerDecoratorWithReturn(Expression, Starters.Expression, Followers.Expression, 6, followers);
                Accept(SymbolEnum.RightRoundBracket);
            }

            return expressionType;
        }

        private void Variable()
        {
            Accept(SymbolEnum.Identifier);
        }

        //private bool IsStartUnsignedConstant(SymbolEnum symbol)
        //{
        //    SymbolEnum[] allowedSymbols = { SymbolEnum.IntConstant, SymbolEnum.RealConstant, SymbolEnum.CharConstant, SymbolEnum.Identifier };
        //    return allowedSymbols.Contains(symbol);
        //}
    }
}
