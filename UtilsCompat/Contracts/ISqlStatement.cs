namespace CarbonCore.Utils.Compat.Contracts
{
    using System.Collections.Generic;
    using System.Data;

    using CarbonCore.Utils.Compat.Database;
    using CarbonCore.Utils.Database;

    public interface ISqlStatement
    {
        SqlStatementType Type { get; }

        ISqlStatement Table(string tableName);

        ISqlStatement Schema(string schemaName);

        ISqlStatement What(string name, string properties = null);

        ISqlStatement WhereConstraint(SqlStatementConstraint constraint);

        ISqlStatement WhereConstraint(IEnumerable<SqlStatementConstraint> constraints);

        ISqlStatement With(string name, object value, string properties = null);

        ISqlStatement OrderBy(SqlStatementOrderByRule rule);

        ISqlStatement OrderBy(IEnumerable<SqlStatementOrderByRule> rules);

        ISqlStatement Output(SqlStatementOutput expression);

        void IntoCommand(IDbCommand target, string statementSuffix = "", bool append = false);
    }
}
