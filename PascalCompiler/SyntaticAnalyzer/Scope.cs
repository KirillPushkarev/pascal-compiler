﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PascalCompiler.SyntaticAnalyzer.TypeDescScalar;

namespace PascalCompiler.SyntaticAnalyzer
{
    public class Scope
    {
        public static Scope CurrentScope { get; private set; }

        public Scope EnclosingScope { get; set; }
        public List<TypeDesc> TypeTable { get; set; }
        public List<IdentifierDesc> IdentifierTable { get; set; }

        public Scope()
        {
            TypeTable = new List<TypeDesc>();
            IdentifierTable = new List<IdentifierDesc>();
        }

        public static void Open()
        {
            var newScope = new Scope();
            newScope.EnclosingScope = CurrentScope;
            CurrentScope = newScope;
        }

        public static void Close()
        {
            CurrentScope = CurrentScope.EnclosingScope;
        }

        public static void CreateInitialScope()
        {
            Open();

            var boolValues = new List<string> { "true", "false" };
            var boolType = new TypeDescEnum(TypeClass.Enums, boolValues);
            var boolIdent = new IdentifierDesc("boolean", IdentifierClass.Types, boolType);
            var trueIdent = new IdentifierDesc("true", IdentifierClass.Consts, boolType);
            var falseIdent = new IdentifierDesc("false", IdentifierClass.Consts, boolType);

            var integerType = new TypeDescScalar(ScalarType.Integer);
            var integerIdent = new IdentifierDesc("integer", IdentifierClass.Types, integerType);

            var realType = new TypeDescScalar(ScalarType.Real);
            var realIdent = new IdentifierDesc("real", IdentifierClass.Types, realType);

            var charType = new TypeDescScalar(ScalarType.Char);
            var charIdent = new IdentifierDesc("char", IdentifierClass.Types, charType);

            CurrentScope.TypeTable.Add(boolType);
            CurrentScope.TypeTable.Add(integerType);
            CurrentScope.TypeTable.Add(realType);
            CurrentScope.TypeTable.Add(charType);
            TypeDesc.booleanType = boolType;
            TypeDesc.integerType = integerType;
            TypeDesc.realType = realType;
            TypeDesc.charType = charType;

            CurrentScope.IdentifierTable.Add(boolIdent);
            CurrentScope.IdentifierTable.Add(trueIdent);
            CurrentScope.IdentifierTable.Add(falseIdent);
            CurrentScope.IdentifierTable.Add(integerIdent);
            CurrentScope.IdentifierTable.Add(realIdent);
            CurrentScope.IdentifierTable.Add(charIdent);
        }

        public IdentifierDesc FindIdentifier(string name)
        {
            var identifier = FindIdentifierInCurrentScope(name);
            if (identifier != null)
                return identifier;

            if (EnclosingScope != null)
                identifier = EnclosingScope.FindIdentifier(name);

            return identifier;
        }

        public IdentifierDesc FindIdentifierInCurrentScope(string name)
        {
            var identifier = IdentifierTable.Find(ident => ident.name == name);
            return identifier;
        }
    }
}
