namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    public static class DataEntrySerialization
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static T Load<T>(byte[] data)
            where T : IDataEntry
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var reader = new BsonReader(stream);
                T instance = Serializer.Deserialize<T>(reader);
                instance.ResetChangedState();
                return instance;
            }
        }

        public static byte[] Save(IDataEntry entry)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BsonWriter(stream);
                Serializer.Serialize(writer, entry);
                stream.Seek(0, SeekOrigin.Begin);

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static byte[] CompactSave(IDataEntry entry)
        {
            using (var stream = new MemoryStream())
            {
                CompactSerialize(entry, stream);
                stream.Seek(0, SeekOrigin.Begin);

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void CompactSerialize(IDataEntry entry, Stream target)
        {
            Type type = entry.GetType();
            DataEntrySerializationDescriptor descriptor = DataEntrySerializationDescriptor.GetDescriptor(type);
            // Todo
        }
    }
}
