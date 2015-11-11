namespace CarbonCore.ContentServices.Logic
{
    using System.Data.SQLite;

    public class SqlLiteDatabaseServiceAction
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlLiteDatabaseServiceAction(SQLiteStatement statement)
        {
            this.Statement = statement;
            this.CanBatch = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SQLiteStatement Statement { get; private set; }

        public SQLiteException Exception { get; set; }

        public bool CanBatch { get; set; }

        public bool WasExecuted { get; set; }

        public bool IgnoreFailure { get; set; }

        public bool Success { get; set; }

        public long ExecutionTime { get; set; }

        public int? Result { get; set; }
    }
}
