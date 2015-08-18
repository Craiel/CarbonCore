namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Utils.Compat;

    public class DataEntrySerializationDescriptor
    {
        private static readonly IDictionary<Type, DataEntrySerializationDescriptor> Descriptors;
        private static readonly IDictionary<Type, DataEntryElementSerializer> Serializers;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DataEntrySerializationDescriptor()
        {
            Descriptors = new Dictionary<Type, DataEntrySerializationDescriptor>();
            Serializers = new Dictionary<Type, DataEntryElementSerializer>();

            RegisterSerializer<bool>(new BooleanSerializer());
            RegisterSerializer<int>(new Int32Serializer());
            RegisterSerializer<long>(new Int64Serializer());
            RegisterSerializer<float>(new FloatSerializer());
            RegisterSerializer<string>(new StringSerializer());
            RegisterSerializer<byte[]>(new ByteArraySerializer());
        }

        public DataEntrySerializationDescriptor(Type type)
        {
            System.Diagnostics.Trace.Assert(typeof(IDataEntry).IsAssignableFrom(type));

            this.Type = type;

            this.Map = new Dictionary<Type, DataSerializationMapEntry>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public IDictionary<Type, DataSerializationMapEntry> Map { get; private set; }
        
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
            var scanQueue = new Queue<DataSerializationMapEntry>();
            scanQueue.Enqueue(new DataSerializationMapEntry(this.Type));
            while (scanQueue.Count > 0)
            {
                var mapEntry = scanQueue.Dequeue();
                this.Map.Add(mapEntry.Type, mapEntry);

                foreach (AttributedPropertyInfo<DataElementAttribute> info in mapEntry.Descriptor.SerializableProperties)
                {
                    var entry = new DataSerializationEntry(mapEntry.Type, info) { Target = info.PropertyType };

                    // Nullable
                    bool isNullable = entry.Target.IsNullable();
                    if (isNullable)
                    {
                        entry.IsNullable = true;
                        entry.Target = entry.Target.GetActualType();
                    }

                    // Check if we have a valid serializer for this entry and add
                    entry.Serializer = this.GetSerializer(entry.Target);
                    if (entry.Serializer != null)
                    {
                        mapEntry.Entries.Add(entry);
                        continue;
                    }
                    
                    // Todo: add Sync<> objects
                    
                    // Everything left over is probably not supported
                    throw new NotImplementedException("Unsupported Type for Serialization: " + entry.Target);
                }
            }
        }

        private DataEntryElementSerializer GetSerializer(Type type)
        {
            // First check if we have a straight up serializer for this type
            if (Serializers.ContainsKey(type))
            {
                return Serializers[type];
            }

            if (typeof(IDataEntry).IsAssignableFrom(type))
            {
                return new DataEntrySerializer(type);
            }

            // Check if this is a generic type
            if (type.IsGenericType)
            {
                return this.GetGenericSerializer(type);
            }

            throw new InvalidOperationException("Could not determine serializer for type " + type);
        }

        private DataEntryElementSerializer GetGenericSerializer(Type type)
        {
            // Check the argument types and gather all required serializers
            Type[] genericArguments = type.GetGenericArguments();
            IDictionary<Type, DataEntryElementSerializer> genericSerializers = new Dictionary<Type, DataEntryElementSerializer>();
            foreach (Type argumentType in genericArguments)
            {
                genericSerializers.Add(argumentType, this.GetSerializer(argumentType));
            }

            // Check if it's a list
            if (genericArguments.Length == 1)
            {
                Type entryType = genericArguments[0];
                Type listType = ListSerializer.BaseListType.MakeGenericType(entryType);
                if (type.IsAssignableFrom(listType))
                {
                    DataEntryElementSerializer serializer = genericSerializers[entryType];
                    return new ListSerializer(entryType, serializer);
                }
            }

            // Check if it's a dictionary
            if (genericArguments.Length == 2)
            {
                Type keyType = genericArguments[0];
                Type valueType = genericArguments[1];
                Type dictionaryType = DictionarySerializer.BaseDictionaryType.MakeGenericType(keyType, valueType);
                if (type.IsAssignableFrom(dictionaryType))
                {
                    return new DictionarySerializer(
                        keyType,
                        valueType,
                        genericSerializers[keyType],
                        genericSerializers[valueType]);
                }
            }

            throw new InvalidOperationException("Could not determine serializer for generic type " + type);
        }
    }
}
