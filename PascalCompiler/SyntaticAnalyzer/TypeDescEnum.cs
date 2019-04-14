using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    public class TypeDescEnum : TypeDesc
    {
        private List<string> values;

        public TypeDescEnum(TypeClass typeClass, List<string> values) : base(typeClass)
        {
            this.values = values;
        }
    }
}
