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

        public static bool AreTypesCompatibleForAddition(TypeDesc type1, TypeDesc type2, SymbolEnum operation)
        {
            if (type1 == null || type2 == null)
                return true;

            switch (operation)
            {
                case SymbolEnum.Plus:
                    return
                        type1 == TypeDesc.integerType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType;
                case SymbolEnum.Minus:
                    return
                        type1 == TypeDesc.integerType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType;
                case SymbolEnum.OrSy:
                    return
                        type1 == TypeDesc.booleanType && type2 == TypeDesc.booleanType;
                default:
                    return false;
            }
        }

        public static bool AreTypesCompatibleForMultiplication(TypeDesc type1, TypeDesc type2, SymbolEnum operation)
        {
            if (type1 == null || type2 == null)
                return true;

            switch (operation)
            {
                case SymbolEnum.Star:
                case SymbolEnum.Slash:
                    return
                        type1 == TypeDesc.integerType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.integerType && type2 == TypeDesc.realType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.integerType ||
                        type1 == TypeDesc.realType && type2 == TypeDesc.realType;
                case SymbolEnum.ModSy:
                case SymbolEnum.DivSy:
                    return
                        type1 == TypeDesc.integerType && type2 == TypeDesc.integerType;
                case SymbolEnum.AndSy:
                    return
                        type1 == TypeDesc.booleanType && type2 == TypeDesc.booleanType;
                default:
                    return false;
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
