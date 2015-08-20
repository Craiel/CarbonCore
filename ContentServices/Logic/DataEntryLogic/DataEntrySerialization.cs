namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
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

        public static IDataEntry CompactLoad(Type type, byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return CompactDeserialize(type, stream);
            }
        }

        public static T CompactLoad<T>(byte[] data)
        {
            return (T)CompactLoad(typeof(T), data);
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
        
        public static void SyncLoad(ISyncEntry instance, byte[] data)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);

                instance.Load(stream);
            }
        }

        public static byte[] SyncSave(ISyncEntry entry)
        {
            using (var stream = new MemoryStream())
            {
                entry.Save(stream);

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
            var context = new SerializationContext(target, entry);
            while (context.Next())
            {
                ContinueSerialize(context);
            }
        }

        private static void ContinueSerialize(SerializationContext context)
        {
            foreach (DataSerializationEntry serializationEntry in context.Descriptor.Entries)
            {
                object value = serializationEntry.Property.GetValue(context.CurrentInstance);
                
                //object value = serializationEntry.Property.Accessor[context.CurrentInstance];

                // If we have a serializer just write the data
                if (serializationEntry.Serializer != null)
                {
                    // Write the type of the serializer
                    if (serializationEntry.IsNullable && value == null)
                    {
                        context.Stream.WriteByte(byte.MaxValue);
                        continue;
                    }

                    // Write the actual content
                    serializationEntry.Serializer.Serialize(context.Stream, value);
                    continue;
                }

                // This entry has no serializer, it has to be a child entry
                context.Enqueue((IDataEntry)value);

                // Todo...
                throw new InvalidDataException("Serialization entry without serializer!");
            }
        }

        private static IDataEntry CompactDeserialize(Type type, Stream source)
        {
            IDataEntry instance = (IDataEntry)Activator.CreateInstance(type);
            var context = new SerializationContext(source, instance);
            while (context.Next())
            {
                ContinueDeserialize(context);
            }

            return instance;
        }

        private static void ContinueDeserialize(SerializationContext context)
        {
            foreach (DataSerializationEntry serializationEntry in context.Descriptor.Entries)
            {
                if (serializationEntry.Serializer != null)
                {
                    object value = serializationEntry.Serializer.Deserialize(context.Stream);
                    serializationEntry.Property.SetValue(context.CurrentInstance, value);
                    continue;
                }

                throw new InvalidDataException("Deserialization entry without serializer!");
            }
        }
        
        private class SerializationContext
        {
            private readonly Queue<IDataEntry> processingQueue;

            public SerializationContext(Stream stream, IDataEntry entry)
            {
                this.Stream = stream;

                this.CurrentType = entry.GetType();
                this.CurrentInstance = entry;

                this.processingQueue = new Queue<IDataEntry>();
                this.processingQueue.Enqueue(entry);

                this.Descriptor = DataEntryDescriptors.GetSerializationDescriptor(this.CurrentType);
            }
            
            public Stream Stream { get; private set; }
            
            public IDataEntry CurrentInstance { get; private set; }

            public Type CurrentType { get; private set; }

            public DataEntrySerializationDescriptor Descriptor { get; private set; }
            
            public bool Next()
            {
                if (this.processingQueue.Count > 0)
                {
                    this.CurrentInstance = this.processingQueue.Dequeue();
                    this.CurrentType = this.CurrentInstance.GetType();
                    return true;
                }

                this.CurrentInstance = null;
                this.CurrentType = null;
                return false;
            }

            public void Enqueue(IDataEntry entry)
            {
                this.processingQueue.Enqueue(entry);
            }
        }
    }
}
