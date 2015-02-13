namespace CarbonCore.Utils.Database
{
    using System;

    public class SqlStatementOutput
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlStatementOutput(string what, string column, Type type)
        {
            this.What = what;
            this.Column = column;
            this.Type = type;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string What { get; private set; }

        public string Column { get; private set; }

        public Type Type { get; private set; }
    }
}
