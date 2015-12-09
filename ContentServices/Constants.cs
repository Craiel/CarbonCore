namespace CarbonCore.ContentServices
{
    public static class Constants
    {
        public const byte SerializationNull = byte.MaxValue;

        public const string StatementTableInfo = "PRAGMA table_info({0})";
        public const string StatementPrimaryKey = "PRIMARY KEY";
        public const string StatementNotNull = "NOT NULL";
        public const string StatementBegin = "BEGIN";
        public const string StatementCommit = "COMMIT";
        public const string StatementRollback = "ROLLBACK";
    }
}
