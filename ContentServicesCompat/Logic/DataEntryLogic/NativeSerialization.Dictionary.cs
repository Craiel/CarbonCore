namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;

    public static partial class NativeSerialization
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void SerializeDictionary<T, TN>(Stream stream, bool isChanged, Dictionary<T, TN> dictionary, SerializationCallbackDelegate<T> keyCallback, SerializationCallbackDelegate<TN> valueCallback)
        {
            if (!WriteHeader(stream, isChanged))
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
            DeserializationCallbackDelegate<T> keyCallback,
            DeserializationCallbackDelegate<TN> valueCallback)
        {
            if (!ReadHeader(source))
            {
                return;
            }

            // Default Dictionaries are always serialized in full
            currentValue.Clear();
            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                var key = keyCallback(source);
                var value = valueCallback(source);
                currentValue.Add(key, value);
            }
        }

        public static void SerializeCascadeDictionary<T, TK, TV>(
            Stream stream,
            SyncCascadeDictionary<T, TK, TV> cascade,
            bool ignoreChangeState) 
            where T : IDictionary<TK, TV> 
            where TK : ISyncEntry 
            where TV : ISyncEntry
        {
            if (!WriteHeader(stream, ignoreChangeState || cascade.IsChanged))
            {
                return;
            }

            if (ignoreChangeState || cascade.IsDictionaryChanged)
            {
                stream.WriteByte(1);

                WriteEnumerableSize(stream, (Int16)cascade.Count);

                // Serialize in full
                foreach (TK key in cascade.Keys)
                {
                    key.Save(stream, true);
                    cascade[key].Save(stream, true);
                }
            }
            else
            {
                stream.WriteByte(0);

                foreach (TK key in cascade.Keys)
                {
                    key.Save(stream);
                    cascade[key].Save(stream);
                }
            }
        }

        public static void DeserializeCascadeDictionary<T, TK, TV>(
            Stream stream,
            SyncCascadeDictionary<T, TK, TV> cascade,
            ConstructionCallbackDelegate<TK> keyConstruction,
            ConstructionCallbackDelegate<TV> valueConstruction)
            where T : IDictionary<TK, TV>
            where TK : ISyncEntry
            where TV : ISyncEntry
        {
            if (!ReadHeader(stream))
            {
                return;
            }

            byte dictionaryIndicator = (byte)stream.ReadByte();
            if (dictionaryIndicator == 1)
            {
                short size = ReadEnumerableSize(stream);
                cascade.Clear();
                for (var i = 0; i < size; i++)
                {
                    var key = keyConstruction();
                    var value = valueConstruction();

                    key.Load(stream);
                    value.Load(stream);

                    cascade.Add(key, value);
                }
            }
            else
            {
                foreach (TK key in cascade.Keys)
                {
                    // Get the instance before deserialization in case the key changes
                    var value = cascade[key];

                    key.Load(stream);
                    value.Load(stream);
                }
            }
        }

        public static void SerializeCascadeKeyDictionary<T, TK, TV>(Stream stream, SyncCascadeKeyDictionary<T, TK, TV> cascade, SerializationCallbackDelegate<TV> valueCallback, bool ignoreChangeState)
            where T : IDictionary<TK, TV>
            where TK : ISyncEntry
        {
            if (!WriteHeader(stream, ignoreChangeState || cascade.IsChanged))
            {
                return;
            }

            if (ignoreChangeState || cascade.IsDictionaryChanged)
            {
                stream.WriteByte(1);

                WriteEnumerableSize(stream, (Int16)cascade.Count);

                // Serialize in full
                foreach (TK key in cascade.Keys)
                {
                    key.Save(stream, true);
                    valueCallback(stream, cascade[key]);
                }
            }
            else
            {
                stream.WriteByte(0);

                foreach (TK key in cascade.Keys)
                {
                    key.Save(stream);
                    valueCallback(stream, cascade[key]);
                }
            }
        }

        public static void DeserializeCascadeKeyDictionary<T, TK, TV>(
            Stream stream,
            SyncCascadeKeyDictionary<T, TK, TV> cascade,
            ConstructionCallbackDelegate<TK> keyConstruction,
            DeserializationCallbackDelegate<TV> valueCallback)
            where T : IDictionary<TK, TV>
            where TK : ISyncEntry
        {
            if (!ReadHeader(stream))
            {
                return;
            }

            byte dictionaryIndicator = (byte)stream.ReadByte();
            if (dictionaryIndicator == 1)
            {
                short size = ReadEnumerableSize(stream);
                cascade.Clear();
                for (var i = 0; i < size; i++)
                {
                    var key = keyConstruction();
                    key.Load(stream);

                    var value = valueCallback(stream);

                    cascade.Add(key, value);
                }
            }
            else
            {
                foreach (TK key in cascade.Keys)
                {
                    key.Load(stream);
                    cascade[key] = valueCallback(stream);
                }
            }
        }

        public static void SerializeCascadeValueDictionary<T, TK, TV>(Stream stream, SyncCascadeValueDictionary<T, TK, TV> cascade, SerializationCallbackDelegate<TK> keyCallback, bool ignoreChangeState)
            where T : IDictionary<TK, TV>
            where TV : ISyncEntry
        {
            if (!WriteHeader(stream, ignoreChangeState || cascade.IsChanged))
            {
                return;
            }

            if (ignoreChangeState || cascade.IsDictionaryChanged)
            {
                stream.WriteByte(1);

                WriteEnumerableSize(stream, (Int16)cascade.Count);

                // Serialize in full
                foreach (TK key in cascade.Keys)
                {
                    keyCallback(stream, key);
                    cascade[key].Save(stream, true);
                }
            }
            else
            {
                stream.WriteByte(0);

                foreach (TK key in cascade.Keys)
                {
                    keyCallback(stream, key);
                    cascade[key].Save(stream);
                }
            }
        }

        public static void DeserializeCascadeValueDictionary<T, TK, TV>(
            Stream stream,
            SyncCascadeValueDictionary<T, TK, TV> cascade,
            DeserializationCallbackDelegate<TK> keyCallback,
            ConstructionCallbackDelegate<TV> valueConstruction)
            where T : IDictionary<TK, TV>
            where TV : ISyncEntry
        {
            if (!ReadHeader(stream))
            {
                return;
            }

            byte dictionaryIndicator = (byte)stream.ReadByte();
            if (dictionaryIndicator == 1)
            {
                short size = ReadEnumerableSize(stream);
                cascade.Clear();
                for (var i = 0; i < size; i++)
                {
                    var key = keyCallback(stream);

                    var value = valueConstruction();
                    value.Load(stream);

                    cascade.Add(key, value);
                }
            }
            else
            {
                for (var i = 0; i < cascade.Count; i++)
                {
                    var key = keyCallback(stream);
                    cascade[key].Load(stream);
                }
            }
        }
    }
}
