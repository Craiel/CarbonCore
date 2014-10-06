namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data;
    using System.Data.SQLite;

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

        public override void IntoCommand(IDbCommand target, string statementSuffix = "")
        {
            string commandText = this.ToString(statementSuffix);

            foreach (string key in this.Values.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(key + statementSuffix, this.Values[key] ?? DBNull.Value));
            }

            foreach (string key in this.Where.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(WhereParameterPrefix + key + statementSuffix, this.Where[key] ?? DBNull.Value));
            }

            // For IN we replace the values directly, anything else would be bad performance wise
            foreach (string key in this.WhereIn.Keys)
            {
                string inValues;

                // Have to escape string values...
                if (this.WhereIn[key][0] is string)
                {
                    inValues = string.Format("'{0}'", string.Join("','", this.WhereIn[key]));
                }
                else
                {
                    inValues = string.Join(",", this.WhereIn[key]);
                }

                commandText = commandText.Replace(WhereInParameterPrefix + key + statementSuffix, inValues);
            }

            if (this.Type == SqlStatementType.Create)
            {
                if (this.DisableRowId)
                {
                    // This will prevent auto increment and auto row id's in sqlite, this is automatic behavior by default
                    // It will cause any INTEGER PRIMARY KEY declaration to behave like auto increment except that it uses a slightly different algorithm
                    commandText += " WITHOUT ROWID";
                }
            }

            if (string.IsNullOrEmpty(target.CommandText))
            {
                target.CommandText = commandText;
            }
            else
            {
                target.CommandText = string.Format("{0}\n{1};", target.CommandText, commandText);
            }
        }
    }
}
