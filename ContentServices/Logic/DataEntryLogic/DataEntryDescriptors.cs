namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.CompactSerializers;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    public static class DataEntryDescriptors
    {
        private static readonly IDictionary<Type, DataEntryDescriptor> Descriptors;
        private static readonly IDictionary<Type, DataEntrySerializationDescriptor> SerializationDescriptors;
        private static readonly IDictionary<Type, DataEntryElementSerializer> Serializers;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DataEntryDescriptors()
        {
            Descriptors = new Dictionary<Type, DataEntryDescriptor>();
            SerializationDescriptors = new Dictionary<Type, DataEntrySerializationDescriptor>();
            Serializers = new Dictionary<Type, DataEntryElementSerializer>();

            RegisterSerializer<bool>(BooleanSerializer.Instance);
            RegisterSerializer<int>(Int32Serializer.Instance);
            RegisterSerializer<long>(Int64Serializer.Instance);
            RegisterSerializer<float>(FloatSerializer.Instance);
            RegisterSerializer<double>(DoubleSerializer.Instance);
            RegisterSerializer<string>(StringSerializer.Instance);
            RegisterSerializer<byte[]>(ByteArraySerializer.Instance);
            RegisterSerializer<float[]>(FloatArraySerializer.Instance);
            RegisterSerializer<double[]>(DoubleArraySerializer.Instance);
            RegisterSerializer<int[]>(Int32ArraySerializer.Instance);
            RegisterSerializer<long[]>(Int64ArraySerializer.Instance);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DataEntryElementSerializer GetSerializer(Type type)
        {
            DataEntryElementSerializer serializer;
            if (Serializers.TryGetValue(type, out serializer))
            {
                return serializer;
            }

            return null;
        }

        public static DataEntryElementSerializer GetCompactSerializer(Type type)
        {
            // First check if we have a straight up serializer for this type
            DataEntryElementSerializer serializer = GetSerializer(type);
            if (serializer != null)
            {
                return serializer;
            }

            if (typeof(IDataEntry).IsAssignableFrom(type))
            {
                return new DataEntrySerializer(type);
            }

            if (type.IsEnum)
            {
                return Int32Serializer.Instance;
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
        
        public static DataEntrySerializationDescriptor GetSerializationDescriptor(Type type)
        {
            lock (Descriptors)
            {
                DataEntrySerializationDescriptor descriptor;
                if (SerializationDescriptors.TryGetValue(type, out descriptor))
                {
                    return descriptor;
                }

                descriptor = new DataEntrySerializationDescriptor(type);
                SerializationDescriptors.Add(type, descriptor);
                return descriptor;
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
                if (genericSerializers.ContainsKey(argumentType))
                {
                    continue;
                }

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
