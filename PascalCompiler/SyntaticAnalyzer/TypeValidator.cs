using static PascalCompiler.Symbol;

namespace PascalCompiler.SyntaticAnalyzer
{
    public class TypeValidator
    {
        public static bool AreTypesAssignmentCompatible(TypeDesc type1, TypeDesc type2)
        {
            return
                type1 == null ||
                type2 == null ||
                type1 == type2 ||
                type1 == TypeDesc.realType && type2 == TypeDesc.integerType;
        }

        public static bool AreTypesComparisonCompatible(TypeDesc type1, TypeDesc type2)
        {
            return
                type1 == null ||
                type2 == null ||
                type1 == type2 ||
                type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                type1 == TypeDesc.charType && type2 == TypeDesc.charType;
        }

        public static bool SupportsSign(TypeDesc type)
        {
            return
                type == null ||
                type == TypeDesc.integerType ||
                type == TypeDesc.realType;
        }

        public static TypeDesc GetTypeAfterAddition(TypeDesc type1, TypeDesc type2, SymbolEnum operation)
        {
            if (type1 == null || type2 == null)
                return null;

            switch (operation)
            {
                case SymbolEnum.Plus:
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.integerType)
                        return TypeDesc.integerType;
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType)
                        return TypeDesc.realType;
                    return null;
                case SymbolEnum.Minus:
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.integerType)
                        return TypeDesc.integerType;
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType)
                        return TypeDesc.realType;
                    return null;
                case SymbolEnum.OrSy:
                    if (type1 == TypeDesc.booleanType && type2 == TypeDesc.booleanType)
                        return TypeDesc.booleanType;
                    return null;
                default:
                    return null;
            }
        }

        public static TypeDesc GetTypeAfterMultiplication(TypeDesc type1, TypeDesc type2, SymbolEnum operation)
        {
            if (type1 == null || type2 == null)
                return null;

            switch (operation)
            {
                case SymbolEnum.Star:
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.integerType)
                        return TypeDesc.integerType;
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType)
                        return TypeDesc.realType;
                    return null;
                case SymbolEnum.Slash:
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType)
                        return TypeDesc.realType;
                    return null;
                case SymbolEnum.ModSy:
                case SymbolEnum.DivSy:
                    if (type1 == TypeDesc.integerType && type2 == TypeDesc.integerType)
                        return TypeDesc.integerType;
                    return null;
                case SymbolEnum.AndSy:
                    if (type1 == TypeDesc.booleanType && type2 == TypeDesc.booleanType)
                        return TypeDesc.booleanType;
                    return null;
                default:
                    return null;
            }
        }

        public static bool IsLogical(TypeDesc type)
        {
            return
                type == null ||
                type == TypeDesc.booleanType;
        }
    }
}
