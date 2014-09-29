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
        public override void IntoCommand(IDbCommand target)
        {
            string commandText = this.ToString();

            foreach (string key in this.Values.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(key, this.Values[key] ?? DBNull.Value));
            }

            foreach (string key in this.Where.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(WhereParameterPrefix + key, this.Where[key] ?? DBNull.Value));
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
                
                commandText = commandText.Replace(WhereInParameterPrefix + key, inValues);
            }

            target.CommandText = commandText;
        }
    }
}
