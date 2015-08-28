namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;

    public delegate void SerializationCallbackDelegate<in T>(Stream target, T value);

    public delegate object DeserializationCallbackDelegate(Stream source);

    public delegate void DeserializationObjectCallbackDelegate<in T>(Stream source, T current);

    public delegate T ConstructionCallbackDelegate<out T>();

    public static class NativeSerialization
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

        public static T Deserialize<T>(Stream source, T currentValue, DeserializationCallbackDelegate callback)
        {
            if (!ReadHeader(source))
            {
                return currentValue;
            }

            return (T)callback(source);
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

        public static T DeserializeCascade<T>(Stream source, T currentValue, ConstructionCallbackDelegate<T> construction) where T : class, ISyncEntry
        {
            object result = currentValue;
            if (!ReadNullableHeader(source, ref result))
            {
                return result as T;
            }

            T typed = result as T;
            if (typed == null)
            {
                typed = construction();
            }

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

        public static void SerializeList<T>(Stream stream, bool isChanged, List<T> list, SerializationCallbackDelegate<T> serializationCallback)
        {
            if (!WriteNullableHeader(stream, isChanged, list))
            {
                return;
            }

            byte[] length = BitConverter.GetBytes((Int16)list.Count);
            stream.Write(length, 0, 2);

            for (var i = 0; i < list.Count; i++)
            {
                serializationCallback(stream, list[i]);
            }
        }

        public static void DeserializeList<T>(
            Stream source,
            List<T> currentValue,
            DeserializationCallbackDelegate deserializationCallback)
        {
            object resultUntyped = currentValue;
            if (!ReadNullableHeader(source, ref resultUntyped))
            {
                return;
            }
            
            // List entries are always serialized in full
            currentValue.Clear();
            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                currentValue.Add((T)deserializationCallback(source));
            }
        }

        public static void SerializeDictionary<T, TN>(Stream stream, bool isChanged, Dictionary<T, TN> dictionary, SerializationCallbackDelegate<T> keyCallback, SerializationCallbackDelegate<TN> valueCallback)
        {
            if (!WriteNullableHeader(stream, isChanged, dictionary))
            {
                return;
            }

            WriteEnumerableSize(stream, (Int16)dictionary.Count);

            foreach (T key in dictionary.Keys)
            {
                keyCallback(stream, key);
                valueCallback(stream, dictionary[key]);
            }
        }

        public static void DeserializeDictionary<T, TN>(
            Stream source,
            Dictionary<T, TN> currentValue,
            DeserializationCallbackDelegate keyCallback,
            DeserializationCallbackDelegate valueCallback)
        {
            object resultUntyped = currentValue;
            if (!ReadNullableHeader(source, ref resultUntyped))
            {
                return;
            }

            // Dictionaries are always serialized in full
            currentValue.Clear();
            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                var key = (T)keyCallback(source);
                var value = (TN)valueCallback(source);
                currentValue.Add(key, value);
            }
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
