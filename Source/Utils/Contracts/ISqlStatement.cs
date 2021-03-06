﻿namespace CarbonCore.Utils.Contracts
{
    using System.Collections.Generic;
    using System.Data;

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

#if !UNITY_5
        void IntoCommand(IDbCommand target, string statementSuffix = "", bool append = false, bool finalize = true);
#endif
    }
}
