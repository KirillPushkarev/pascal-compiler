using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    public class TypeDescScalar : TypeDesc
    {
        public enum ScalarType
        {
            Integer,
            Real,
            Char
        }

        public ScalarType scalarType;

        public TypeDescScalar(ScalarType scalarType) : base(TypeClass.Scalars)
        {
            this.scalarType = scalarType;
        }
    }
}
