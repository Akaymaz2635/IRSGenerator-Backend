namespace IRSGenerator.Core;

public static class Constants
{
    public static class Role
    {
        public const string Admin      = "Admin";
        public const string UserWriter = "UserWriter";
        public const string UserReader = "UserReader";
    }

    public static class Claim
    {
        public static class Value
        {
            // IRSProject
            public const string IRSProjectWrite = "irsproject.write";
            public const string IRSProjectRead  = "irsproject.read";

            // Character
            public const string CharacterWrite = "character.write";
            public const string CharacterRead  = "character.read";

            // CategoricalPartResult
            public const string CategoricalPartResultWrite = "categoricalpartresult.write";
            public const string CategoricalPartResultRead  = "categoricalpartresult.read";

            // CategoricalZoneResult
            public const string CategoricalZoneResultWrite = "categoricalzoneresult.write";
            public const string CategoricalZoneResultRead  = "categoricalzoneresult.read";

            // NumericalPartResult
            public const string NumericalPartResultWrite = "numericalpartresult.write";
            public const string NumericalPartResultRead  = "numericalpartresult.read";

            // Authorization
            public const string AuthorizationWrite = "authorization.write";
            public const string AuthorizationRead  = "authorization.read";
        }
    }
}
