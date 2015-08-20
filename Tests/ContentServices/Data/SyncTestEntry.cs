namespace CarbonCore.Tests.ContentServices
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Tests.ContentServices.Data;

    public class SyncTestEntry : SyncEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncTestEntry()
        {
            this.Id = new Sync<int?>();
            this.TestInt = new Sync<int>();
            this.TestLong = new Sync<long>();
            this.TestFloat = new Sync<float>();
            this.TestBool = new Sync<bool>();
            this.ByteArray = new Sync<byte[]>();
            this.TestString = new Sync<string>();
            this.Enum = new Sync<TestEnum>();

            this.CascadedEntry = new Sync<SyncTestEntry2>();

            this.SimpleCollection = new SyncList<List<int>, int>();
            this.CascadingCollection = new SyncList<List<SyncTestEntry2>, SyncTestEntry2>();

            this.SimpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            this.CascadingDictionary = new SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Sync<int?> Id { get; set; }

        public Sync<int> TestInt { get; set; }

        public Sync<long> TestLong { get; set; }

        public Sync<float> TestFloat { get; set; }

        public Sync<bool> TestBool { get; set; }

        public Sync<byte[]> ByteArray { get; set; }

        public Sync<string> TestString { get; set; }

        public Sync<TestEnum> Enum { get; set; }

        public Sync<SyncTestEntry2> CascadedEntry { get; set; }

        public SyncList<List<int>, int> SimpleCollection { get; set; }

        public SyncList<List<SyncTestEntry2>, SyncTestEntry2> CascadingCollection { get; set; }

        public SyncDictionary<Dictionary<string, float>, string, float> SimpleDictionary { get; set; }

        public SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2> CascadingDictionary { get; set; }
        
        public override void Save(Stream target)
        {
            // Simple types
            NativeSerialization.SerializeObject(target, this.Id.IsChanged, this.Id.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.TestInt.IsChanged, this.TestInt.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.TestLong.IsChanged, this.TestLong.Value, Int64Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.TestFloat.IsChanged, this.TestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.TestBool.IsChanged, this.TestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.ByteArray.IsChanged, this.ByteArray.Value, ByteArraySerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.TestString.IsChanged, this.TestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, this.Enum.IsChanged, this.Enum.Value, (stream, value) => Int32Serializer.Instance.Serialize(stream, (int)value));

            // Cascaded objects
            NativeSerialization.SerializeObject(target, this.CascadedEntry.IsChanged, this.CascadedEntry.Value, (stream, value) => value.Save(stream));

            // Lists
            NativeSerialization.SerializeList(target, this.SimpleCollection.IsChanged, this.SimpleCollection.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.SerializeList(target, this.CascadingCollection.IsChanged, this.CascadingCollection.Value, (stream, value) => value.Save(stream));
            
            // Dictionaries
            NativeSerialization.SerializeDictionary(target, this.SimpleDictionary.IsChanged, this.SimpleDictionary.Value, StringSerializer.Instance.Serialize, FloatSerializer.Instance.Serialize);
            NativeSerialization.SerializeDictionary(target, this.CascadingDictionary.IsChanged, this.CascadingDictionary.Value, Int32Serializer.Instance.Serialize, (stream, value) => value.Save(stream));
        }

        public override void Load(Stream source)
        {
            this.Id = NativeSerialization.Deserialize(source, this.Id.Value, Int32Serializer.Instance.Deserialize);
            this.TestInt = NativeSerialization.Deserialize(source, this.TestInt.Value, Int32Serializer.Instance.Deserialize);
            this.TestLong = NativeSerialization.Deserialize(source, this.TestLong.Value, Int64Serializer.Instance.Deserialize);
            this.TestFloat = NativeSerialization.Deserialize(source, this.TestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.TestBool = NativeSerialization.Deserialize(source, this.TestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.ByteArray = NativeSerialization.Deserialize(source, this.ByteArray.Value, ByteArraySerializer.Instance.Deserialize);
            this.TestString = NativeSerialization.Deserialize(source, this.TestString.Value, StringSerializer.Instance.Deserialize);
            this.Enum = NativeSerialization.Deserialize(source, this.Enum.Value, Int32Serializer.Instance.Deserialize);

            this.CascadedEntry = NativeSerialization.DeserializeObject(
                source,
                this.CascadedEntry.Value,
                () => new SyncTestEntry2(),
                (stream, current) => current.Load(stream));

            this.SimpleCollection = NativeSerialization.DeserializeList(
                    source,
                    this.SimpleCollection.Value,
                    () => new List<int>(),
                    Int32Serializer.Instance.Deserialize);

            this.CascadingCollection =
                NativeSerialization.DeserializeList(
                    source,
                    this.CascadingCollection.Value,
                    () => new List<SyncTestEntry2>(),
                    stream =>
                    {
                        var entry = new SyncTestEntry2();
                        entry.Load(stream);
                        return entry;
                    });

            this.SimpleDictionary = NativeSerialization.DeserializeDictionary(
                source,
                this.SimpleDictionary.Value,
                () => new Dictionary<string, float>(),
                StringSerializer.Instance.Deserialize,
                FloatSerializer.Instance.Deserialize);

            this.CascadingDictionary = NativeSerialization.DeserializeDictionary(
                source,
                this.CascadingDictionary.Value,
                () => new Dictionary<int, SyncTestEntry2>(),
                Int32Serializer.Instance.Deserialize,
                stream =>
                    {
                        var entry = new SyncTestEntry2();
                        entry.Load(stream);
                        return entry;
                    });
        }

        public override void ResetSyncState(bool state = false)
        {
            this.Id.ResetChangeState(state);
            this.TestInt.ResetChangeState(state);
            this.TestLong.ResetChangeState(state);
            this.TestFloat.ResetChangeState(state);
            this.TestBool.ResetChangeState(state);
            this.ByteArray.ResetChangeState(state);
            this.TestString.ResetChangeState(state);
            this.Enum.ResetChangeState(state);

            this.CascadedEntry.ResetChangeState(state);

            this.SimpleCollection.ResetChangeState(state);
            this.CascadingCollection.ResetChangeState(state);
            if (this.CascadingCollection.Value != null)
            {
                foreach (SyncTestEntry2 entry in this.CascadingCollection)
                {
                    entry.ResetSyncState(state);
                }
            }

            this.SimpleDictionary.ResetChangeState(state);
            this.CascadingDictionary.ResetChangeState(state);
            if (this.CascadingDictionary.Value != null)
            {
                foreach (int key in this.CascadingDictionary.Keys)
                {
                    this.CascadingDictionary[key].ResetSyncState(state);
                }
            }
        }
    }
}
