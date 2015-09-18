namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;

    public static partial class NativeSerialization
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void SerializeList<T>(Stream stream, bool isChanged, List<T> list, SerializationCallbackDelegate<T> serializationCallback)
        {
            if (!WriteHeader(stream, isChanged))
            {
                return;
            }

            WriteEnumerableSize(stream, (short)list.Count);

            for (var i = 0; i < list.Count; i++)
            {
                serializationCallback(stream, list[i]);
            }
        }

        public static void DeserializeList<T>(
            Stream source,
            List<T> currentValue,
            DeserializationCallbackDelegate<T> deserializationCallback)
        {
            if (!ReadHeader(source))
            {
                return;
            }

            // List entries are always serialized in full
            currentValue.Clear();
            short length = ReadEnumerableSize(source);
            for (var i = 0; i < length; i++)
            {
                currentValue.Add(deserializationCallback(source));
            }
        }

        public static void SerializeCascadeList<T, TN>(Stream stream, SyncCascadeList<T, TN> cascade, bool ignoreChangeState)
            where T : List<TN>
            where TN : ISyncEntry
        {
            if (!WriteHeader(stream, ignoreChangeState || cascade.IsChanged))
            {
                return;
            }

            if (ignoreChangeState || cascade.IsListChanged)
            {
                stream.WriteByte(1);

                // The list was changed so we have to serialize in full
                WriteEnumerableSize(stream, (short)cascade.Value.Count);

                for (var i = 0; i < cascade.Value.Count; i++)
                {
                    cascade.Value[i].Save(stream, true);
                }
            }
            else
            {
                stream.WriteByte(0);

                // The list itself was not modified, just update the entries
                for (var i = 0; i < cascade.Value.Count; i++)
                {
                    cascade.Value[i].Save(stream);
                }
            }
        }

        public static void DeserializeCascadeList<T, TN>(Stream stream, SyncCascadeList<T, TN> cascade)
            where T : List<TN>
            where TN : ISyncEntry, new()
        {
            DeserializeCascadeList(stream, cascade, () => new TN());
        }

        public static void DeserializeCascadeList<T, TN>(Stream stream, SyncCascadeList<T, TN> cascade, ConstructionCallbackDelegate<TN> construction)
            where T : List<TN>
            where TN : ISyncEntry
        {
            if (!ReadHeader(stream))
            {
                return;
            }

            byte listStateMarker = (byte)stream.ReadByte();
            if (listStateMarker == 1)
            {
                // Deserialize in full
                cascade.Value.Clear();

                short count = ReadEnumerableSize(stream);
                for (var i = 0; i < count; i++)
                {
                    TN entry = construction();
                    entry.Load(stream);
                    cascade.Add(entry);
                }
            }
            else
            {
                // Deserialize element differences
                for (var i = 0; i < cascade.Value.Count; i++)
                {
                    cascade.Value[i].Load(stream);
                }
            }
        }
    }
}
