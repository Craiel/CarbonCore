namespace CarbonCore.ContentServices.Sql.Logic.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class DatabaseEntryAttribute : Attribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryAttribute(string table)
        {
            this.Table = table;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Table { get; private set; }
    }
}
