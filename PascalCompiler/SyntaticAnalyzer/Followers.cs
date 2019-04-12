using System.Collections.Generic;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler.SyntaticAnalyzer
{
    public static class Followers
    {
        public static HashSet<SymbolEnum> Block => new HashSet<SymbolEnum> { SymbolEnum.Dot };

        public static HashSet<SymbolEnum> LabelDaclarationPart { get; set; } = new HashSet<SymbolEnum> { SymbolEnum.ConstSy, SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> ConstantDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.TypeSy, SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> TypeDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.VarSy, SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> VarDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.FunctionSy, SymbolEnum.ProcedureSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> ProcAndFuncDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> StatementPart => new HashSet<SymbolEnum> { SymbolEnum.Dot };

        public static HashSet<SymbolEnum> VarDeclaration => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> Type => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> ArrayType => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> Statement => new HashSet<SymbolEnum> { SymbolEnum.Dot, SymbolEnum.Semicolon, SymbolEnum.EndSy };

        public static HashSet<SymbolEnum> CompoundStatement => new HashSet<SymbolEnum> { SymbolEnum.Dot, SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> SimpleStatement => AssignmentStatement;

        public static HashSet<SymbolEnum> AssignmentStatement => new HashSet<SymbolEnum> { SymbolEnum.Semicolon, SymbolEnum.EndSy };

        public static HashSet<SymbolEnum> Expression => SimpleExpression;

        public static HashSet<SymbolEnum> SimpleExpression => new HashSet<SymbolEnum> { SymbolEnum.Semicolon, SymbolEnum.EndSy, SymbolEnum.Equals, SymbolEnum.NotEquals, SymbolEnum.Less, SymbolEnum.LessEquals, SymbolEnum.GreaterEquals, SymbolEnum.Greater, SymbolEnum.InSy, SymbolEnum.RightRoundBracket };

        public static HashSet<SymbolEnum> Term => 
            new HashSet<SymbolEnum>
            {
                SymbolEnum.Semicolon,
                SymbolEnum.EndSy,
                SymbolEnum.Equals,
                SymbolEnum.NotEquals,
                SymbolEnum.Less,
                SymbolEnum.LessEquals,
                SymbolEnum.GreaterEquals,
                SymbolEnum.Greater,
                SymbolEnum.InSy,
                SymbolEnum.RightRoundBracket,
                SymbolEnum.Minus,
                SymbolEnum.Plus,
                SymbolEnum.OfSy,
                SymbolEnum.Star,
                SymbolEnum.Slash,
                SymbolEnum.DivSy,
                SymbolEnum.ModSy,
                SymbolEnum.AndSy
            };

        public static HashSet<SymbolEnum> Factor => 
            new HashSet<SymbolEnum>
            {
                SymbolEnum.Semicolon,
                SymbolEnum.EndSy,
                SymbolEnum.Equals,
                SymbolEnum.NotEquals,
                SymbolEnum.Less,
                SymbolEnum.LessEquals,
                SymbolEnum.GreaterEquals,
                SymbolEnum.Greater,
                SymbolEnum.InSy,
                SymbolEnum.RightRoundBracket,
                SymbolEnum.Minus,
                SymbolEnum.Plus,
                SymbolEnum.OfSy,
                SymbolEnum.Star,
                SymbolEnum.Slash,
                SymbolEnum.DivSy,
                SymbolEnum.ModSy,
                SymbolEnum.AndSy
            };
    }
}
