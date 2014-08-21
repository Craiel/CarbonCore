namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;

    public enum PrimaryKeyMode
    {
        None,
        Assigned,
        AutoIncrement
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseEntryPrimaryKeyAttribute : DatabaseEntryElementAttribute
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryPrimaryKeyAttribute()
        {
        }

        public DatabaseEntryPrimaryKeyAttribute(string name)
            : base(name)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public PrimaryKeyMode PrimaryKey { get; set; }
    }
}
