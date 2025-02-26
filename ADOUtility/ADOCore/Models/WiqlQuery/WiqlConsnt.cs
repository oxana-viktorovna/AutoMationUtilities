namespace ADOCore.Models.WiqlQuery
{
    public static class WiqlConsnt
    {
        public static class Conjunction
        {
            public const string And = "AND";
            public const string AndNot = "AND NOT";
            public const string Or = "OR";
            public const string OrNot = "OR NOT";
        }

        public static class Operator
        {
            public const string Equal = "=";
            public const string NotEqual = "<>";
            public const string Greater = ">";
            public const string Less = "<";
            public const string EqualOrGrater = ">=";
            public const string EqualOrLess = "<=";
            public const string In = "IN";
            public const string Contains = "Contains";
            public const string Under = "Under";
        }
    }
}
