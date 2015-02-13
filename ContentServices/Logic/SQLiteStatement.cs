namespace CarbonCore.ContentServices.Logic
{
    using System.Data;

    using CarbonCore.Utils.Database;

    public class SQLiteStatement : SqlStatement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SQLiteStatement(SqlStatementType type)
            : base(type)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool DisableRowId { get; set; }

        public override void IntoCommand(IDbCommand target, string statementSuffix = "", bool append = false)
        {
            base.IntoCommand(target, statementSuffix, append);

            if (this.Type == SqlStatementType.Create)
            {
                if (this.DisableRowId)
                {
                    // This will prevent auto increment and auto row id's in sqlite, this is automatic behavior by default
                    // It will cause any INTEGER PRIMARY KEY declaration to behave like auto increment except that it uses a slightly different algorithm
                    target.CommandText += " WITHOUT ROWID";
                }
            }
        }
    }
}
