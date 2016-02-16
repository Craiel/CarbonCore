namespace CarbonCore.ContentServices.Sql.Logic.Attributes
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;

    public enum PrimaryKeyMode
    {
        None,
        Autoincrement,
        Assigned,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseEntryElementAttribute : DataElementAttribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryElementAttribute()
        {
        }

        public DatabaseEntryElementAttribute(string name)
        {
            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public PrimaryKeyMode PrimaryKeyMode { get; set; }
    }
}
