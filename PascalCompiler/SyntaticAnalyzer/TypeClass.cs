using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    public enum TypeClass
    {
        Scalars,
        Limiteds,
        Enums,
        Arrays,
        References,
        Sets,
        Files,
        Records
    }
}
