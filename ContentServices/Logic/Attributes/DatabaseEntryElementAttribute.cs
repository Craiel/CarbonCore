namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;

    public enum PrimaryKeyMode
    {
        None,
        Autoincrement,
        Assigned,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class DatabaseEntryElementAttribute : Attribute
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
