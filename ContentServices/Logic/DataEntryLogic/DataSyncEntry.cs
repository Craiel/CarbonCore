namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Reflection;

    public class DataSyncEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataSyncEntry(Type type, PropertyInfo property)
        {
            this.Type = type;
            this.Property = property;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public PropertyInfo Property { get; private set; }
    }
}
