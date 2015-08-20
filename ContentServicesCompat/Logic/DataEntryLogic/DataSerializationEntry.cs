namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Compat.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class DataSerializationEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataSerializationEntry(Type source, AttributedPropertyInfo<DataElementAttribute> property)
        {
            this.Source = source;
            this.Property = property;

            this.ChildEntries = new List<DataSerializationEntry>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<DataSerializationEntry> ChildEntries { get; private set; } 

        public Type Source { get; private set; }

        public AttributedPropertyInfo<DataElementAttribute> Property { get; private set; }

        public DataEntryElementSerializer Serializer { get; set; }

        public bool IsNullable { get; set; }

        public Type Target { get; set; }
    }
}
