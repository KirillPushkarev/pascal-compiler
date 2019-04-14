using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PascalCompiler.SyntaticAnalyzer
{
    public class IdentifierDesc
    {
        public string name;
        public IdentifierClass identifierClass;
        public TypeDesc type;

        public IdentifierDesc(string name, IdentifierClass identifierClass)
        {
            this.name = name;
            this.identifierClass = identifierClass;
        }

        public IdentifierDesc(string name, IdentifierClass identifierClass, TypeDesc type)
        {
            this.name = name;
            this.identifierClass = identifierClass;
            this.type = type;
        }
    }
}
