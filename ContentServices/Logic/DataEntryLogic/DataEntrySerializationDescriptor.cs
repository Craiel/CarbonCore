namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class DataEntrySerializationDescriptor
    {
        private static readonly IDictionary<Type, DataEntrySerializationDescriptor> Descriptors;
        private static readonly IDictionary<Type, DataEntryElementSerializer> Serializers;

        private IDictionary<DataSerializationKey, DataSerializationEntry> entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DataEntrySerializationDescriptor()
        {
            Descriptors = new Dictionary<Type, DataEntrySerializationDescriptor>();
            Serializers = new Dictionary<Type, DataEntryElementSerializer>();
        }

        public DataEntrySerializationDescriptor(Type type)
        {
            System.Diagnostics.Trace.Assert(typeof(IDataEntry).IsAssignableFrom(type));

            this.Type = type;

            this.entries = new Dictionary<DataSerializationKey, DataSerializationEntry>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }
        
        public static DataEntrySerializationDescriptor GetDescriptor(Type type)
        {
            lock (Descriptors)
            {
                if (!Descriptors.ContainsKey(type))
                {
                    Descriptors.Add(type, new DataEntrySerializationDescriptor(type));
                }

                return Descriptors[type];
            }
        }

        public static void RegisterSerializer<T>(DataEntryElementSerializer serializer)
        {
            Serializers.Add(typeof(T), serializer);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            var scanQueue = new Queue<Type>();
            scanQueue.Enqueue(this.Type);
            while (scanQueue.Count > 0)
            {
                Type type = scanQueue.Dequeue();

                DataEntryDescriptor descriptor = DataEntryDescriptor.GetDescriptor(type);
                foreach (AttributedPropertyInfo<DataElementAttribute> info in descriptor.SerializableProperties)
                {
                    var entry = new DataSerializationEntry(type, info) { Target = info.Property.PropertyType };
                    
                    // Todo: add Sync<> objects

                    // Check if this property is another data object
                    if (typeof(IDataEntry).IsAssignableFrom(entry.Target))
                    {
                        entry.IsDataEntry = true;
                        this.entries.Add(entry.Key, entry);
                        scanQueue.Enqueue(entry.Target);
                        continue;
                    }

                    // Nullable
                    bool isNullable = entry.Target.IsNullable();
                    if (isNullable)
                    {
                        entry.IsNullable = true;
                        entry.Target = entry.Target.GetActualType();
                    }

                    // Check if we have a valid serializer for this entry
                    if (Serializers.ContainsKey(entry.Target))
                    {
                        this.entries.Add(entry.Key, entry);
                        continue;
                    }

                    // Check if this is a primitive type
                    /*if (entry.Target.IsPrimitive || entry.Target == typeof(string))
                    {
                        this.entries.Add(key, entry);
                        continue;
                    }*/

                    // Generic support (lists & dictionaries
                    /*if (entry.Target.IsGenericType)
                    {
                        Type[] genericArguments = entry.Target.GetGenericArguments();
                    }*/
                    
                    // Everything left over is probably not supported
                    throw new NotImplementedException("Unsupported Type for Serialization: " + entry.Target);
                }
            }
        }
    }
}
