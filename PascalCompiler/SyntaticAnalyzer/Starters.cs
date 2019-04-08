using System.Collections.Generic;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler.SyntaticAnalyzer
{
    public static class Starters
    {
        public static HashSet<SymbolEnum> Block => new HashSet<SymbolEnum> { SymbolEnum.LabelSy, SymbolEnum.ConstSy, SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> LabelDaclarationPart { get; set; } = new HashSet<SymbolEnum> { SymbolEnum.LabelSy, SymbolEnum.ConstSy, SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> ConstantDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.ConstSy, SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> TypeDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> VarDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> ProcAndFuncDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> StatementPart => CompoundStatement;

        public static HashSet<SymbolEnum> VarDeclaration => new HashSet<SymbolEnum> { SymbolEnum.Identifier };

        public static HashSet<SymbolEnum> Type => new HashSet<SymbolEnum> { SymbolEnum.IntConstant, SymbolEnum.CharConstant, SymbolEnum.Identifier, SymbolEnum.ArraySy, SymbolEnum.RecordSy, SymbolEnum.SetSy, SymbolEnum.FileSy, SymbolEnum.Arrow };

        public static HashSet<SymbolEnum> Statement => new HashSet<SymbolEnum>(CompoundStatement.Concat(SimpleStatement));

        public static HashSet<SymbolEnum> CompoundStatement => new HashSet<SymbolEnum> { SymbolEnum.BeginSy };

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
                SymbolEnum.NilSy,
                SymbolEnum.LeftRoundBracket
            };
    }
}
