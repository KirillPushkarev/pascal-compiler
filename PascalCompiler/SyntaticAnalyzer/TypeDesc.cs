using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    abstract public class TypeDesc
    {
        public static TypeDesc booleanType;
        public static TypeDesc integerType;
        public static TypeDesc realType;
        public static TypeDesc charType;

        public TypeClass typeClass;

        public TypeDesc(TypeClass typeClass)
        {
            this.typeClass = typeClass;
        }
    }
}
