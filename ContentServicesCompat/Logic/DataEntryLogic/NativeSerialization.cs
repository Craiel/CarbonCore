namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;

    public delegate void SerializationCallbackDelegate<in T>(Stream target, T value);

    public delegate T DeserializationCallbackDelegate<out T>(Stream source);

    public delegate void DeserializationObjectCallbackDelegate<in T>(Stream source, T current);

    public delegate T ConstructionCallbackDelegate<out T>();

    public delegate T EnumConversionCallback<out T>(int value);

    public static partial class NativeSerialization
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Serialize<T>(Stream stream, bool isChanged, T value, SerializationCallbackDelegate<T> serializationCallback)
        {
            if (!WriteHeader(stream, isChanged))
            {
                return;
            }

            serializationCallback(stream, value);
        }

        public static T Deserialize<T>(Stream source, T currentValue, DeserializationCallbackDelegate<T> callback)
        {
            if (!ReadHeader(source))
            {
                return currentValue;
            }

            return callback(source);
        }
        
        public static void SerializeObject<T>(Stream stream, bool isChanged, T value, SerializationCallbackDelegate<T> callback)
        {
            if (!WriteNullableHeader(stream, isChanged, value))
            {
                return;
            }

            callback(stream, value);
        }

        public static T DeserializeObject<T>(
            Stream source,
            T currentValue,
            ConstructionCallbackDelegate<T> construction,
            DeserializationObjectCallbackDelegate<T> callback)
            where T : class
        {
            object result = currentValue;
            if (!ReadNullableHeader(source, ref result))
            {
                return result as T;
            }

            if (result == null)
            {
                result = construction();
            }

            callback(source, (T)result);
            return (T)result;
        }

        public static void SerializeEnum<T>(Stream stream, bool isChanged, T value)
            where T : struct, IConvertible
        {
            if (!WriteHeader(stream, isChanged))
            {
                return;
            }

            // GetHashCode returns the actual int value for enums, we rely on that being true
            Int32Serializer.Instance.Serialize(stream, value.GetHashCode());
        }

        public static T DeserializeEnum<T>(Stream source, T currentValue, EnumConversionCallback<T> conversion)
            where T : struct, IConvertible
        {
            if (!ReadHeader(source))
            {
                return currentValue;
            }

            return conversion(Int32Serializer.Instance.Deserialize(source));
        }

        public static void SerializeCascade<T>(Stream stream, SyncCascade<T> host, bool ignoreChangeState)
            where T : class, ISyncEntry
        {
            if (!WriteNullableHeader(stream, ignoreChangeState || host.IsChanged, host.Value))
            {
                return;
            }
            
            // If the source reference changed we also have to cascade everything
            host.Value.Save(stream, host.IsReferenceChanged || ignoreChangeState);
        }

        public static T DeserializeCascade<T>(Stream source, T currentValue) where T : class, ISyncEntry, new()
        {
            return DeserializeCascade(source, currentValue, () => new T());
        }

        public static T DeserializeCascade<T>(Stream source, T currentValue, ConstructionCallbackDelegate<T> construction) where T : class, ISyncEntry
        {
            object result = currentValue;
            if (!ReadNullableHeader(source, ref result))
            {
                return result as T;
            }

            T typed = result as T ?? construction();

            typed.Load(source);
            return typed;
        }

        public static void SerializeCascadeReadOnly<T>(Stream stream, SyncCascadeReadOnly<T> host, bool ignoreChangeState)
            where T : class, ISyncEntry
        {
            if (!WriteHeader(stream, ignoreChangeState || host.IsChanged))
            {
                return;
            }

            host.Value.Save(stream, ignoreChangeState);
        }

        public static void DeserializeCascadeReadOnly<T>(Stream source, T currentValue) where T : class, ISyncEntry
        {
            if (!ReadHeader(source))
            {
                return;
            }

            currentValue.Load(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static bool WriteHeader(Stream stream, bool isChanged)
        {
            if (!isChanged)
            {
                stream.WriteByte(0);
                return false;
            }

            stream.WriteByte(1);
            return true;
        }

        private static bool WriteNullableHeader(Stream stream, bool isChanged, object value)
        {
            if (!WriteHeader(stream, isChanged))
            {
                return false;
            }

            if (value == null)
            {
                stream.WriteByte(Constants.SerializationNull);
                return false;
            }

            stream.WriteByte(1);
            return true;
        }

        private static bool ReadHeader(Stream source)
        {
            return source.ReadByte() == 1;
        }

        private static bool ReadNullableHeader(Stream source, ref object result)
        {
            if (!ReadHeader(source))
            {
                return false;
            }

            if (source.ReadByte() == Constants.SerializationNull)
            {
                result = null;
                return false;
            }

            return true;
        }

        private static void WriteEnumerableSize(Stream target, Int16 count)
        {
            byte[] length = BitConverter.GetBytes(count);
            target.Write(length, 0, 2);
        }

        private static Int16 ReadEnumerableSize(Stream source)
        {
            byte[] lengthData = new byte[2];
            source.Read(lengthData, 0, 2);

            return BitConverter.ToInt16(lengthData, 0);
        }
    }
}
