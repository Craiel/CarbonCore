namespace CarbonCore.Utils.Compat.Database
{
    public class SqlStatementOrderByRule
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlStatementOrderByRule(string column, bool ascending = false)
        {
            this.Column = column;

            this.Ascending = ascending;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Column { get; private set; }

        public bool Ascending { get; private set; }
    }
}
