using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    public enum IdentifierClass
    {
        Progs,
        Types,
        Consts,
        Vars,
        Procs,
        Funcs,
        Unknown
    }
}
