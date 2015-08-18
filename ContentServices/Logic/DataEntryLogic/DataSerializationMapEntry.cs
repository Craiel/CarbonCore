namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    public class DataSerializationMapEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataSerializationMapEntry(Type type)
        {
            this.Type = type;
            this.ChildMapEntries = new List<DataSerializationMapEntry>();
            this.Entries = new List<DataSerializationEntry>();

            this.Descriptor = DataEntryDescriptor.GetDescriptor(type);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public IList<DataSerializationMapEntry> ChildMapEntries { get; private set; }

        public IList<DataSerializationEntry> Entries { get; private set; }

        public DataEntryDescriptor Descriptor { get; private set; }
    }
}
