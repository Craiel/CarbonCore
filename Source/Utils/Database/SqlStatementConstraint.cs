namespace CarbonCore.Utils.Database
{
    using System.Collections.Generic;

    public class SqlStatementConstraint
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlStatementConstraint(string column, object value, SqlStatementConstraintType type = SqlStatementConstraintType.Equals)
        {
            this.Column = column;
            this.Values = new List<object> { value };
            this.Type = type;
        }

        public SqlStatementConstraint(string column, IEnumerable<object> value)
        {
            this.Column = column;
            this.Values = new List<object>(value);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool Negate { get; set; }

        public string Column { get; private set; }

        public IList<object> Values { get; private set; }

        public SqlStatementConstraintType Type { get; private set; }
    }
}
