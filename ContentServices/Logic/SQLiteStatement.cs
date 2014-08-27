namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data.Common;
    using System.Data.SQLite;

    using CarbonCore.Utils.Database;

    public class SQLiteStatement : SqlStatement
    {
        public SQLiteStatement(SqlStatementType type)
            : base(type)
        {
        }

        public override void IntoCommand(DbCommand target)
        {
            target.CommandText = this.ToString();

            foreach (string key in this.Values.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(key, this.Values[key] ?? DBNull.Value));
            }

            foreach (string key in this.Where.Keys)
            {
                target.Parameters.Add(new SQLiteParameter(WhereParameterPrefix + key, this.Where[key] ?? DBNull.Value));
            }
        }
    }
}
