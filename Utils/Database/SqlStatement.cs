namespace CarbonCore.Utils.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;
    
    public interface ISqlStatement
    {
        SqlStatementType Type { get; }
    }

    public class SqlStatement : ISqlStatement
    {
        protected const string WhereParameterPrefix = "@WHR_";
        protected const string WhereInParameterPrefix = "@WHRI_";

        private readonly IList<string> what;
        private readonly IDictionary<string, string> whatProperties;

        private readonly IList<string> order;

        private readonly IDictionary<string, object> values;
        private readonly IDictionary<string, string> valueProperties;

        private readonly IDictionary<string, object> where;
        private readonly IDictionary<string, IList<object>> whereIn;

        private string table;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlStatement(SqlStatementType type)
        {
            this.Type = type;

            this.what = new List<string>();
            this.whatProperties = new Dictionary<string, string>();
            this.order = new List<string>();
            this.values = new Dictionary<string, object>();
            this.valueProperties = new Dictionary<string, string>();
            this.where = new Dictionary<string, object>();
            this.whereIn = new Dictionary<string, IList<object>>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SqlStatementType Type { get; private set; }

        public SqlStatement Table(string tableName)
        {
            this.table = tableName;
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

        public SqlStatement OrderBy(string name)
        {
            System.Diagnostics.Trace.Assert(!this.order.Contains(name));

            this.order.Add(name);
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

        public SqlStatement WhereConstraint(string name, object value)
        {
            this.where.Add(name, value);
            return this;
        }

        public SqlStatement InConstraint(string name, IList<object> inValues)
        {
            System.Diagnostics.Trace.Assert(inValues != null && inValues.Count > 0);

            this.whereIn.Add(name, inValues);
            return this;
        }
        
        public override string ToString()
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
                        return this.BuildDelete();
                    }

                case SqlStatementType.Insert:
                    {
                        return this.BuildInsert();
                    }

                case SqlStatementType.Select:
                    {
                        return this.BuildSelect();
                    }

                case SqlStatementType.Update:
                    {
                        return this.BuildUpdate();
                    }

                default:
                    {
                        throw new NotImplementedException(this.Type.ToString());
                    }
            }
        }

        public virtual void IntoCommand(DbCommand target)
        {
            throw new NotImplementedException();
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

        protected IDictionary<string, object> Where
        {
            get
            {
                return this.where;
            }
        }

        protected IDictionary<string, IList<object>> WhereIn
        {
            get
            {
                return this.whereIn;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string BuildDrop()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));

            return string.Format("DROP TABLE {0}", this.table);
        }

        private string BuildCreate()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));

            var builder = new StringBuilder();
            builder.AppendFormat("CREATE TABLE {0}", this.table);

            IList<string> whatSegments = new List<string>();
            foreach (string name in this.what)
            {
                System.Diagnostics.Trace.Assert(this.whatProperties.ContainsKey(name));

                whatSegments.Add(string.Format("{0} {1}", name, this.whatProperties[name]));
            }

            builder.AppendFormat("({0})", string.Join(",", whatSegments));
            return builder.ToString();
        }

        private string BuildDelete()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));

            var builder = new StringBuilder();
            builder.AppendFormat("DELETE FROM {0}", this.table);

            builder.Append(this.BuildWhereSegment());

            return builder.ToString();
        }

        private string BuildSelect()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));

            var builder = new StringBuilder("SELECT ");

            builder.Append(this.what.Count > 0 ? string.Join(",", this.what) : "*");

            builder.AppendFormat(" FROM {0}", this.table);

            builder.Append(this.BuildWhereSegment());
            builder.Append(this.BuildOrderSegment());

            return builder.ToString();
        }

        private string BuildInsert()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));

            var builder = new StringBuilder();
            builder.AppendFormat("INSERT INTO {0}", this.table);

            builder.AppendFormat("({0})", string.Join(",", this.values.Keys));

            builder.AppendFormat(" VALUES (@{0})", string.Join(",@", this.values.Keys));

            return builder.ToString();
        }

        private string BuildUpdate()
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(this.table));
            System.Diagnostics.Trace.Assert(this.where.Count > 0);

            var builder = new StringBuilder();
            builder.AppendFormat("UPDATE {0}", this.table);

            builder.AppendFormat(" SET ");
            IList<string> segments = new List<string>();
            foreach (string key in this.values.Keys)
            {
                segments.Add(string.Format("{0} = @{0}", key));
            }

            builder.Append(string.Join(",", segments));
            builder.Append(this.BuildWhereSegment());

            return builder.ToString();
        }

        private string BuildWhereSegment()
        {
            if (this.where.Count <= 0 && this.whereIn.Count <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            builder.Append(" WHERE ");
            IList<string> segments = new List<string>();
            foreach (string key in this.where.Keys)
            {
                segments.Add(string.Format("{0} = {1}", key, WhereParameterPrefix + key));
            }
            foreach (string key in this.whereIn.Keys)
            {
                segments.Add(string.Format("{0} IN ({1})", key, WhereInParameterPrefix + key));
            }

            builder.Append(string.Join(" AND ", segments));

            return builder.ToString();
        }

        private string BuildOrderSegment()
        {
            if (this.order.Count <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(" ORDER BY ");
            builder.Append(string.Join(",", this.order));

            return builder.ToString();
        }
    }
}
