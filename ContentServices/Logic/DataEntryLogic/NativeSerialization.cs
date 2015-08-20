namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    
    public delegate long SerializationCallbackDelegate<in T>(Stream target, T value);

    public delegate object DeserializationCallbackDelegate(Stream source);

    public delegate object DeserializationObjectCallbackDelegate<T>(Stream source, T current);

    public delegate object ConstructionCallbackDelegate();

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
            ConstructionCallbackDelegate construction,
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
            return result as T;
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

        public static List<T> DeserializeList<T>(
            Stream source,
            List<T> currentValue,
            ConstructionCallbackDelegate construction,
            DeserializationCallbackDelegate deserializationCallback)
        {
            object resultUntyped = currentValue;
            if (!ReadNullableHeader(source, ref resultUntyped))
            {
                return resultUntyped as List<T>;
            }

            List<T> result = resultUntyped as List<T>;
            if (result == null)
            {
                result = (List<T>)construction();
            }

            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                result.Add((T)deserializationCallback(source));
            }

            return result;
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

        public static Dictionary<T, TN> DeserializeDictionary<T, TN>(
            Stream source,
            Dictionary<T, TN> currentValue,
            ConstructionCallbackDelegate construction,
            DeserializationCallbackDelegate keyCallback,
            DeserializationCallbackDelegate valueCallback)
        {
            object resultUntyped = currentValue;
            if (!ReadNullableHeader(source, ref resultUntyped))
            {
                return resultUntyped as Dictionary<T, TN>;
            }

            Dictionary<T, TN> result = resultUntyped as Dictionary<T, TN>;
            if (result == null)
            {
                result = (Dictionary<T, TN>)construction();
            }

            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                var key = (T)keyCallback(source);
                var value = (TN)valueCallback(source);
                result.Add(key, value);
            }

            return result;
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
                stream.WriteByte(byte.MaxValue);
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

            if (source.ReadByte() == 0)
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
