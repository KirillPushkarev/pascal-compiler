using System.Collections.Generic;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler.SyntaticAnalyzer
{
    public static class Starters
    {
        public static HashSet<SymbolEnum> Program => new HashSet<SymbolEnum> { SymbolEnum.ProgramSy };

        public static HashSet<SymbolEnum> ProgramHeading => new HashSet<SymbolEnum> { SymbolEnum.ProgramSy };

        public static HashSet<SymbolEnum> Block => new HashSet<SymbolEnum> { SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> TypeDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> VarDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.VarSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> StatementPart => new HashSet<SymbolEnum> { SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> VarDeclaration => new HashSet<SymbolEnum> { SymbolEnum.Identifier };

        public static HashSet<SymbolEnum> Type => new HashSet<SymbolEnum> { SymbolEnum.Minus, SymbolEnum.Plus, SymbolEnum.IntConstant, SymbolEnum.RealConstant, SymbolEnum.CharConstant, SymbolEnum.Identifier, SymbolEnum.ArraySy };

        public static HashSet<SymbolEnum> SubrangeType => Constant;

        public static HashSet<SymbolEnum> ArrayType => new HashSet<SymbolEnum> { SymbolEnum.ArraySy };

        public static HashSet<SymbolEnum> Constant => new HashSet<SymbolEnum> { SymbolEnum.Minus, SymbolEnum.Plus, SymbolEnum.IntConstant, SymbolEnum.RealConstant, SymbolEnum.Identifier, SymbolEnum.CharConstant };

        public static HashSet<SymbolEnum> Statement => new HashSet<SymbolEnum>(StructuredStatement.Concat(SimpleStatement));

        public static HashSet<SymbolEnum> StructuredStatement => new HashSet<SymbolEnum>(CompoundStatement.Concat(ConditionalStatement).Concat(RepetitiveStatement));

        public static HashSet<SymbolEnum> CompoundStatement => new HashSet<SymbolEnum> { SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> ConditionalStatement => new HashSet<SymbolEnum> { SymbolEnum.IfSy };

        public static HashSet<SymbolEnum> RepetitiveStatement => new HashSet<SymbolEnum> { SymbolEnum.WhileSy };

        public static HashSet<SymbolEnum> SimpleStatement => AssignmentStatement;

        public static HashSet<SymbolEnum> AssignmentStatement => new HashSet<SymbolEnum> { SymbolEnum.Identifier };

        public static HashSet<SymbolEnum> Expression => SimpleExpression;

        public static HashSet<SymbolEnum> SimpleExpression => new HashSet<SymbolEnum>(new HashSet<SymbolEnum> { SymbolEnum.Minus, SymbolEnum.Plus }.Concat(Term));

        public static HashSet<SymbolEnum> Term => Factor;

        public static HashSet<SymbolEnum> Factor => 
            new HashSet<SymbolEnum>{
                SymbolEnum.NotSy,
                SymbolEnum.Identifier,
                SymbolEnum.IntConstant,
                SymbolEnum.RealConstant,
                SymbolEnum.CharConstant,
                SymbolEnum.LeftRoundBracket
            };
    }
}
