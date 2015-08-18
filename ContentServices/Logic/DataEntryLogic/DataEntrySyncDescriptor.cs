namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.Utils.Compat;

    public class DataEntrySyncDescriptor
    {
        private static readonly Type SyncBaseType = typeof(SyncContent<>);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataEntrySyncDescriptor(Type type)
        {
            this.Type = type;

            this.Entries = new List<DataSyncEntry>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public IList<DataSyncEntry> Entries { get; private set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            IList<PropertyInfo> properties = this.Type.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.IsGenericType)
                {
                    System.Diagnostics.Trace.TraceWarning("Unknown type in Sync<>: {0}", property.PropertyType);
                    continue;
                }

                if (property.PropertyType.GenericTypeArguments.Length > 1)
                {
                    System.Diagnostics.Trace.TraceWarning("Unknown Generic type in Sync<>: {0}", property.PropertyType);
                    continue;
                }

                Type innerType = property.PropertyType.GenericTypeArguments[0];
                if (innerType.IsNullable())
                {
                    innerType = innerType.GetActualType();
                }

                DataEntryElementSerializer serializer = DataEntryDescriptors.GetSerializer(innerType);
                if (serializer == null)
                {
                    System.Diagnostics.Trace.TraceWarning("No serializer for type {0}", innerType);
                    continue;
                }

                Type syncType = SyncBaseType.MakeGenericType(innerType);
                if (!syncType.IsAssignableFrom(property.PropertyType))
                {
                    System.Diagnostics.Trace.TraceWarning("Type mismatch, expected SyncContent<> but got {0}", syncType);
                    continue;
                }

                var entry = new DataSyncEntry(syncType, property);
                DataEntrySyncDescriptors.SetInnerSyncSerializer(syncType, serializer);
                this.Entries.Add(entry);
            }
        }
    }
}
