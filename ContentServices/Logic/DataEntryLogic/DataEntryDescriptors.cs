namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    public static class DataEntryDescriptors
    {
        private static readonly IDictionary<Type, DataEntryDescriptor> Descriptors;
        private static readonly IDictionary<Type, DataEntrySerializationDescriptor> SerializationDescriptors;
        private static readonly IDictionary<Type, DataEntrySyncDescriptor> SyncDescriptors;
        private static readonly IDictionary<Type, DataEntryElementSerializer> Serializers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DataEntryDescriptors()
        {
            Descriptors = new Dictionary<Type, DataEntryDescriptor>();
            SerializationDescriptors = new Dictionary<Type, DataEntrySerializationDescriptor>();
            SyncDescriptors = new Dictionary<Type, DataEntrySyncDescriptor>();
            Serializers = new Dictionary<Type, DataEntryElementSerializer>();

            RegisterSerializer<bool>(new BooleanSerializer());
            RegisterSerializer<int>(new Int32Serializer());
            RegisterSerializer<long>(new Int64Serializer());
            RegisterSerializer<float>(new FloatSerializer());
            RegisterSerializer<string>(new StringSerializer());
            RegisterSerializer<byte[]>(new ByteArraySerializer());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DataEntryElementSerializer GetSerializer(Type type)
        {
            if (Serializers.ContainsKey(type))
            {
                return Serializers[type];
            }

            return null;
        }

        public static DataEntryElementSerializer GetCompactSerializer(Type type)
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
                return GetGenericCompactSerializer(type);
            }

            throw new InvalidOperationException("Could not determine serializer for type " + type);
        }

        public static DataEntryDescriptor GetDescriptor<T>()
        {
            return GetDescriptor(typeof(T));
        }

        public static DataEntryDescriptor GetDescriptor(Type type)
        {
            lock (Descriptors)
            {
                if (!Descriptors.ContainsKey(type))
                {
                    Descriptors.Add(type, new DataEntryDescriptor(type));
                }

                return Descriptors[type];
            }
        }

        public static DataEntrySyncDescriptor GetSyncDescriptor(Type type)
        {
            lock (SyncDescriptors)
            {
                if (!SyncDescriptors.ContainsKey(type))
                {
                    SyncDescriptors.Add(type, new DataEntrySyncDescriptor(type));
                }

                return SyncDescriptors[type];
            }
        }

        public static DataEntrySerializationDescriptor GetSerializationDescriptor(Type type)
        {
            lock (Descriptors)
            {
                if (!SerializationDescriptors.ContainsKey(type))
                {
                    SerializationDescriptors.Add(type, new DataEntrySerializationDescriptor(type));
                }

                return SerializationDescriptors[type];
            }
        }

        public static void RegisterSerializer<T>(DataEntryElementSerializer serializer)
        {
            Serializers.Add(typeof(T), serializer);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static DataEntryElementSerializer GetGenericCompactSerializer(Type type)
        {
            // Check the argument types and gather all required serializers
            Type[] genericArguments = type.GetGenericArguments();
            IDictionary<Type, DataEntryElementSerializer> genericSerializers = new Dictionary<Type, DataEntryElementSerializer>();
            foreach (Type argumentType in genericArguments)
            {
                genericSerializers.Add(argumentType, GetCompactSerializer(argumentType));
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
