﻿namespace CarbonCore.Utils.Database
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Text;
    
    public interface ISqlStatement
    {
        SqlStatementType Type { get; }
    }

    public class SqlStatement : ISqlStatement
    {
        protected const string WhereParameterPrefix = "@WHR_";
        protected const string WhereInParameterPrefix = "@WHRI_";
        protected const string InsertOutputTempTable = "@tmp_InsertOutput";

        private readonly IList<string> what;
        private readonly IDictionary<string, string> whatProperties;

        private readonly List<SqlStatementOrderByRule> order;
        private readonly List<SqlStatementConstraint> where;
        private readonly List<SqlStatementOutput> output;

        private readonly IDictionary<string, object> values;
        private readonly IDictionary<string, string> valueProperties;

        private string table;
        private string schema;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlStatement(SqlStatementType type)
        {
            this.Type = type;

            this.what = new List<string>();
            this.whatProperties = new Dictionary<string, string>();
            this.order = new List<SqlStatementOrderByRule>();
            this.values = new Dictionary<string, object>();
            this.valueProperties = new Dictionary<string, string>();
            this.where = new List<SqlStatementConstraint>();
            this.output = new List<SqlStatementOutput>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SqlStatementType Type { get; private set; }

        public ReadOnlyCollection<SqlStatementOutput> OutputRules
        {
            get
            {
                return this.output.AsReadOnly();
            }
        }

        public static void PrepareCommand(IDbCommand command, IList<SqlStatement> statements)
        {
            System.Diagnostics.Trace.Assert(statements.Count > 0);

            var builder = new StringBuilder();

            IList<string> insertOutput = new List<string>();
            foreach (SqlStatement statement in statements)
            {
                switch (statement.Type)
                {
                    case SqlStatementType.Insert:
                        {
                            if (statement.output.Count > 0)
                            {
                                foreach (SqlStatementOutput output in statement.output)
                                {
                                    if (!insertOutput.Contains(output.Column))
                                    {
                                        insertOutput.Add(string.Format("{0} {1}", output.Column, GetDatabaseTypeString(output.Type)));
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            if (insertOutput.Count > 0)
            {
                builder.AppendFormat("DECLARE {0} table ({1})", InsertOutputTempTable, string.Join(",", insertOutput));
            }

            builder.AppendLine();
            command.CommandText = builder.ToString();
        }

        public static void FinalizeCommand(IDbCommand command, IList<SqlStatement> statements)
        {
            System.Diagnostics.Trace.Assert(statements.Count > 0);

            var commandAppendix = new StringBuilder();
            foreach (SqlStatement statement in statements)
            {
                switch (statement.Type)
                {
                    case SqlStatementType.Insert:
                        {
                            commandAppendix.AppendLine(string.Concat("SELECT * FROM ", InsertOutputTempTable, ";"));
                            break;
                        }
                }
            }

            command.CommandText = string.Concat(command.CommandText, "\n", commandAppendix.ToString());
        }

        public SqlStatement Table(string tableName)
        {
            this.table = tableName;
            return this;
        }

        public SqlStatement Schema(string schemaName)
        {
            this.schema = schemaName;
            return this;
        }

        public SqlStatement What(string name, string properties = null)
        {
            System.Diagnostics.Trace.Assert(!this.what.Contains(name));

            this.what.Add(name);
            if (!string.IsNullOrEmpty(properties))
            {
                this.whatProperties.Add(name, properties);
            }

            return this;
        }

        public SqlStatement OrderBy(SqlStatementOrderByRule rule)
        {
            this.order.Add(rule);
            return this;
        }

        public SqlStatement OrderBy(IEnumerable<SqlStatementOrderByRule> rules)
        {
            this.order.AddRange(rules);
            return this;
        }

        public SqlStatement With(string name, object value, string properties = null)
        {
            System.Diagnostics.Trace.Assert(!this.values.ContainsKey(name));

            this.values.Add(name, value);
            if (!string.IsNullOrEmpty(properties))
            {
                this.valueProperties.Add(name, properties);
            }

            return this;
        }

        public SqlStatement WhereConstraint(SqlStatementConstraint constraint)
        {
            this.where.Add(constraint);
            return this;
        }

        public SqlStatement WhereConstraint(IEnumerable<SqlStatementConstraint> constraints)
        {
            this.where.AddRange(constraints);
            return this;
        }

        public SqlStatement Output(SqlStatementOutput expression)
        {
            this.output.Add(expression);
            return this;
        }

        public override string ToString()
        {
            return this.ToString(string.Empty);
        }

        public virtual void IntoCommand(IDbCommand target, string statementSuffix = "", bool append = false)
        {
            string commandText = string.Concat(this.ToString(statementSuffix), ";");

            foreach (string key in this.Values.Keys)
            {
                IDbDataParameter parameter = target.CreateParameter();
                parameter.ParameterName = key + statementSuffix;
                parameter.Value = this.Values[key] ?? DBNull.Value;
                target.Parameters.Add(parameter);
            }

            foreach (SqlStatementConstraint constraint in this.Where)
            {
                if (constraint.Values.Count > 1)
                {
                    // For IN we replace the values directly, anything else would be bad performance wise
                    string inValues;

                    // Have to escape string values...
                    if (constraint.Values[0] is string)
                    {
                        inValues = string.Format("'{0}'", string.Join("','", constraint.Values));
                    }
                    else
                    {
                        inValues = string.Join(",", constraint.Values);
                    }

                    commandText = commandText.Replace(WhereInParameterPrefix + constraint.Column + statementSuffix, inValues);
                    continue;
                }

                IDbDataParameter parameter = target.CreateParameter();
                parameter.ParameterName = WhereParameterPrefix + constraint.Column + statementSuffix;
                parameter.Value = constraint.Values[0] ?? DBNull.Value;
                target.Parameters.Add(parameter);
            }

            if (string.IsNullOrEmpty(target.CommandText) || !append)
            {
                target.CommandText = commandText;
            }
            else
            {
                target.CommandText = string.Format("{0}\n{1}", target.CommandText, commandText);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected IDictionary<string, object> Values
        {
            get
            {
                return this.values;
            }
        }

        protected IList<SqlStatementConstraint> Where
        {
            get
            {
                return this.where;
            }
        }

        protected string ToString(string suffix)
        {
            switch (this.Type)
            {
                case SqlStatementType.Drop:
                    {
                        return this.BuildDrop();
                    }

                case SqlStatementType.Create:
                    {
                        return this.BuildCreate();
                    }

                case SqlStatementType.Delete:
                    {
                        return this.BuildDelete(suffix);
                    }

                case SqlStatementType.Insert:
                    {
                        return this.BuildInsert(suffix);
                    }

                case SqlStatementType.Select:
                    {
                        return this.BuildSelect();
                    }

                case SqlStatementType.Update:
                    {
                        return this.BuildUpdate(suffix);
                    }

                case SqlStatementType.Use:
                    {
                        return this.BuildUse();
                    }

                default:
                    {
                        return Diagnostics.Internal.NotImplemented<string>(this.Type.ToString());
                    }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static string GetDatabaseTypeString(Type internalType)
        {
            if (internalType == typeof(int))
            {
                return "ING";
            }

            if (internalType == typeof(long))
            {
                return "BIGINT";
            }

            if (internalType == typeof(string))
            {
                return "VARCHAR(4096)";
            }

            return Diagnostics.Internal.NotImplemented<string>("GetDatabaseType: " + internalType);
        }

        private string GetFullTableName()
        {
            if (string.IsNullOrEmpty(this.schema))
            {
                return this.table;
            }

            return string.Concat(this.schema, ".", this.table);
        }

        private string BuildDrop()
        {
            string tableName = this.GetFullTableName();
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(tableName));

            return string.Format("DROP TABLE {0}", tableName);
        }

        private string BuildCreate()
        {
            string tableName = this.GetFullTableName();
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(tableName));

            var builder = new StringBuilder();
            builder.AppendFormat("CREATE TABLE {0}", tableName);

            IList<string> whatSegments = new List<string>();
            foreach (string name in this.what)
            {
                System.Diagnostics.Trace.Assert(this.whatProperties.ContainsKey(name));

                whatSegments.Add(string.Format("{0} {1}", name, this.whatProperties[name]));
            }

            builder.AppendFormat("({0})", string.Join(",", whatSegments));
            return builder.ToString();
        }

        private string BuildDelete(string suffix)
        {
            string tableName = this.GetFullTableName();
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(tableName));

            var builder = new StringBuilder();
            builder.AppendFormat("DELETE FROM {0}", tableName);

            builder.Append(this.BuildWhereSegment(suffix));

            return builder.ToString();
        }

        private string BuildSelect()
        {
            string tableName = this.GetFullTableName();

            var builder = new StringBuilder("SELECT ");

            builder.Append(this.what.Count > 0 ? string.Join(",", this.what) : "*");

            if (!string.IsNullOrEmpty(tableName))
            {
                builder.AppendFormat(" FROM {0}", tableName);
            }

            builder.Append(this.BuildWhereSegment(string.Empty));
            builder.Append(this.BuildOrderSegment());

            return builder.ToString();
        }

        private string BuildInsert(string suffix)
        {
            string tableName = this.GetFullTableName();
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(tableName));

            var builder = new StringBuilder();
            builder.AppendFormat("INSERT INTO {0}", tableName);

            builder.AppendFormat("({0})", string.Join(",", this.values.Keys));

            if (this.output.Count > 0)
            {
                IList<string> outputExpressions = new List<string>();
                foreach (SqlStatementOutput statementOutput in this.output)
                {
                    outputExpressions.Add(statementOutput.What);
                }

                builder.AppendFormat(" OUTPUT {0} INTO {1}", string.Join(",", outputExpressions), InsertOutputTempTable);
            }

            IList<string> valueKeys = new List<string>();
            foreach (string key in this.values.Keys)
            {
                valueKeys.Add(key + suffix);
            }

            builder.AppendFormat(" VALUES (@{0})", string.Join(",@", valueKeys));

            return builder.ToString();
        }

        private string BuildUpdate(string suffix)
        {
            string tableName = this.GetFullTableName();
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(tableName));
            System.Diagnostics.Trace.Assert(this.where.Count > 0);

            var builder = new StringBuilder();
            builder.AppendFormat("UPDATE {0}", tableName);

            builder.AppendFormat(" SET ");
            IList<string> segments = new List<string>();
            foreach (string key in this.values.Keys)
            {
                segments.Add(string.Format("{0} = @{0}{1}", key, suffix));
            }

            builder.Append(string.Join(",", segments));
            builder.Append(this.BuildWhereSegment(suffix));

            return builder.ToString();
        }

        private string BuildUse()
        {
            System.Diagnostics.Trace.Assert(this.what.Count == 1, "Need to specify target for Use statement!");

            return string.Format("USE [{0}]", this.what[0]);
        }

        private string BuildWhereSegment(string suffix)
        {
            if (this.where.Count <= 0)
            {
                return string.Empty;
            }

            IList<string> segments = new List<string>();
            foreach (SqlStatementConstraint constraint in this.where)
            {
                var segment = new StringBuilder();
                if (constraint.Negate)
                {
                    segment.Append(" NOT ");
                }

                if (constraint.Values.Count > 1)
                {
                    segment.AppendFormat("{0} IN ({1})", constraint.Column, WhereInParameterPrefix + constraint.Column + suffix);
                    continue;
                }

                string sign = null;
                switch (constraint.Type)
                {
                    case SqlStatementConstraintType.Equals:
                        {
                            sign = "=";
                            break;
                        }

                    case SqlStatementConstraintType.GreaterThen:
                        {
                            sign = ">";
                            break;
                        }

                    case SqlStatementConstraintType.GreaterThenEquals:
                        {
                            sign = ">=";
                            break;
                        }

                    case SqlStatementConstraintType.LessThen:
                        {
                            sign = "<";
                            break;
                        }

                    case SqlStatementConstraintType.LessThenEquals:
                        {
                            sign = "<=";
                            break;
                        }

                    default:
                        {
                            return Diagnostics.Internal.NotImplemented<string>(constraint.Type.ToString());
                        }
                }

                segments.Add(string.Format("{0} {1} {2}", constraint.Column, sign, WhereParameterPrefix + constraint.Column + suffix));
            }

            return string.Format(" WHERE {0}", string.Join(" AND ", segments));
        }

        private string BuildOrderSegment()
        {
            if (this.order.Count <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(" ORDER BY ");
            IList<string> segments = new List<string>();
            foreach (SqlStatementOrderByRule rule in this.order)
            {
                segments.Add(string.Format("{0} {1}", rule.Column, rule.Ascending ? "ASC" : "DESC"));
            }

            builder.Append(string.Join(",", segments));

            return builder.ToString();
        }
    }
}
