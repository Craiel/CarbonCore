namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;

    public class DataEntrySerializationDescriptor
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataEntrySerializationDescriptor(Type type)
        {
            Debug.Assert(typeof(IDataEntry).IsAssignableFrom(type));

            this.Type = type;

            this.Entries = new List<DataSerializationEntry>();
            
            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public IList<DataSerializationEntry> Entries { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            var descriptor = DataEntryDescriptors.GetDescriptor(this.Type);

            foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.SerializableProperties)
            {
                var entry = new DataSerializationEntry(this.Type, info) { Target = info.PropertyType };

                // Nullable
                bool isNullable = entry.Target.IsNullable();
                if (isNullable)
                {
                    entry.IsNullable = true;
                    entry.Target = entry.Target.GetActualType();
                }

                // Check if we have a valid serializer for this entry and add
                entry.Serializer = DataEntryDescriptors.GetCompactSerializer(entry.Target);
                if (entry.Serializer != null)
                {
                    this.Entries.Add(entry);
                    continue;
                }
                
                // Everything left over is probably not supported
                throw new NotSupportedException("Unsupported Type for Serialization: " + entry.Target);
            }
        }
    }
}
