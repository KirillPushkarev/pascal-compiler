using System.Collections.Generic;
using System.Linq;
using static PascalCompiler.Symbol;

namespace PascalCompiler.SyntaticAnalyzer
{
    public static class Followers
    {
        public static HashSet<SymbolEnum> Program => new HashSet<SymbolEnum> {};

        public static HashSet<SymbolEnum> ProgramHeading => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> Block => new HashSet<SymbolEnum> { SymbolEnum.Dot };

        public static HashSet<SymbolEnum> TypeDefinitionPart => new HashSet<SymbolEnum> { SymbolEnum.VarSy, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> VarDeclarationPart => new HashSet<SymbolEnum> { SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> StatementPart => new HashSet<SymbolEnum> { SymbolEnum.Dot };

        public static HashSet<SymbolEnum> VarDeclaration => new HashSet<SymbolEnum> { SymbolEnum.Identifier, SymbolEnum.BeginSy };

        public static HashSet<SymbolEnum> Type => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> SubrangeType => new HashSet<SymbolEnum> { SymbolEnum.Semicolon, SymbolEnum.RightSquareBracket };

        public static HashSet<SymbolEnum> ArrayType => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> Statement => new HashSet<SymbolEnum> { SymbolEnum.Dot, SymbolEnum.Semicolon, SymbolEnum.EndSy };

        public static HashSet<SymbolEnum> StructuredStatement => new HashSet<SymbolEnum>(CompoundStatement.Concat(ConditionalStatement).Concat(RepetitiveStatement));

        public static HashSet<SymbolEnum> CompoundStatement => new HashSet<SymbolEnum> { SymbolEnum.Dot, SymbolEnum.Semicolon, SymbolEnum.EndSy };

        public static HashSet<SymbolEnum> ConditionalStatement => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> RepetitiveStatement => new HashSet<SymbolEnum> { SymbolEnum.Semicolon };

        public static HashSet<SymbolEnum> SimpleStatement => AssignmentStatement;

        public static HashSet<SymbolEnum> AssignmentStatement => new HashSet<SymbolEnum> { SymbolEnum.Semicolon, SymbolEnum.EndSy };

        public static HashSet<SymbolEnum> Expression => SimpleExpression;

        public static HashSet<SymbolEnum> SimpleExpression => 
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
                SymbolEnum.ThenSy,
                SymbolEnum.DoSy
            };

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
                SymbolEnum.AndSy,
                SymbolEnum.ThenSy,
                SymbolEnum.DoSy
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
                SymbolEnum.AndSy,
                SymbolEnum.ThenSy,
                SymbolEnum.DoSy
            };
    }
}
