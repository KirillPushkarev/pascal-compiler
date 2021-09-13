﻿using System.Collections.Generic;

namespace PascalCompiler
{
    public class Symbol
    {
        public enum SymbolEnum
        {
            Star,
            Slash,
            Equals,
            NotEquals,
            Comma,
            Semicolon,
            Colon,
            Dot,
            Arrow,
            LeftRoundBracket,
            RightRoundBracket,
            LeftSquareBracket,
            RightSquareBracket,
            LeftCurlyBracket,
            RightCurlyBracket,
            Less,
            Greater,
            LessEquals,
            GreaterEquals,
            Plus,
            Minus,
            LeftComment,
            RightComment,
            Assign,
            TwoDots,
            Identifier,
            RealConstant,
            IntConstant,
            CharConstant,

            // Reserved words
            AndSy,
            ArraySy,
            AsmSy,
            BeginSy,
            BreakSy,
            CaseSy,
            ConstSy,
            ConstructorSy,
            ContinueSy,
            DestructorSy,
            DivSy,
            DoSy,
            DowntoSy,
            ElseSy,
            EndSy,
            FileSy,
            ForSy,
            FunctionSy,
            GotoSy,
            IfSy,
            ImplementationSy,
            InSy,
            InlineSy,
            InterfaceSy,
            LabelSy,
            ModSy,
            NilSy,
            NotSy,
            ObjectSy,
            OfSy,
            OnSy,
            OperatorSy,
            OrSy,
            PackedSy,
            ProcedureSy,
            ProgramSy,
            RecordSy,
            RepeatSy,
            SetSy,
            ShlSy,
            ShrSy,
            ThenSy,
            ToSy,
            TypeSy,
            UnitSy,
            UntilSy,
            UsesSy,
            VarSy,
            WhileSy,
            WithSy,
            XorSy,
        }

        public static Dictionary<string, SymbolEnum> keywordMapping = new Dictionary<string, SymbolEnum>()
        {
            { "array", SymbolEnum.ArraySy },
            { "begin", SymbolEnum.BeginSy },
            { "div", SymbolEnum.DivSy },
            { "do", SymbolEnum.DoSy },
            { "else", SymbolEnum.ElseSy },
            { "end", SymbolEnum.EndSy },
            { "for", SymbolEnum.ForSy },
            { "if", SymbolEnum.IfSy },
            { "in", SymbolEnum.InSy },
            { "mod", SymbolEnum.ModSy },
            { "of", SymbolEnum.OfSy },
            { "procedure", SymbolEnum.ProcedureSy },
            { "program", SymbolEnum.ProgramSy },
            { "to", SymbolEnum.ToSy },
            { "type", SymbolEnum.TypeSy },
            { "var", SymbolEnum.VarSy },
            { "while", SymbolEnum.WhileSy }
        };

        public static Dictionary<SymbolEnum, int> symbolToErrorCodeMapping = new Dictionary<SymbolEnum, int>()
        {
            { SymbolEnum.Identifier, 2 },
            { SymbolEnum.ProgramSy, 3 },
            { SymbolEnum.RightRoundBracket, 4 },
            { SymbolEnum.Colon, 5 },
            { SymbolEnum.OfSy, 8 },
            { SymbolEnum.LeftRoundBracket, 9 },
            { SymbolEnum.LeftSquareBracket, 11 },
            { SymbolEnum.RightSquareBracket, 12 },
            { SymbolEnum.EndSy, 13 },
            { SymbolEnum.Semicolon, 14 },
            { SymbolEnum.IntConstant, 15 },
            { SymbolEnum.Equals, 16 },
            { SymbolEnum.BeginSy, 17 },
            { SymbolEnum.Comma, 20 },
            { SymbolEnum.Assign, 51 },
            { SymbolEnum.ThenSy, 52 },
            { SymbolEnum.UntilSy, 53 },
            { SymbolEnum.DoSy, 54 },
            { SymbolEnum.IfSy, 56 },
            { SymbolEnum.ToSy, 57 },
            { SymbolEnum.DowntoSy, 58 },
            { SymbolEnum.Dot, 61 },
            { SymbolEnum.TwoDots, 74 },
            { SymbolEnum.CharConstant, 83 },
        };
    }
}