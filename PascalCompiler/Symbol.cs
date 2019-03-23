using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler
{
    enum Symbol
    {
        Star,
        Slash,
        Equals,
        NotEquals,
        Comma,
        Semicolon,
        Colon,
        Point,
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
        TwoPoints,
        Identifier,
        FLoatConstant,
        IntConstant,
        CharConstant,

        CaseSy,
        ElseSy,
        FileSy,
        GotoSy,
        ThenSy,
        TypeSy,
        UntilSy,
        DoSy,
        WithSy,
        IfSy,
        InSy,
        OfSy,
        ProgramSy,
        VarSy,
        ProcedureSy,
        BeginSy,
        EndSy,


        IntegerSy,
        ArraySy,
        RealSy,
        CharSy,

        TrueSy,
        FalseSy,
    }
}
