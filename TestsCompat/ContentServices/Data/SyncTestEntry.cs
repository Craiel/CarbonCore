namespace CarbonCore.Tests.ContentServices
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Tests.ContentServices.Data;

    public class SyncTestEntry : SyncEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncTestEntry()
        {
            this.Id = new SyncNull<int?>();
            this.TestInt = new Sync<int>();
            this.TestLong = new Sync<long>();
            this.TestFloat = new Sync<float>();
            this.TestBool = new Sync<bool>();
            this.ByteArray = new SyncObject<byte[]>();
            this.TestString = new SyncObject<string>();
            this.Enum = new Sync<TestEnum>();

            this.CascadedEntry = new SyncCascade<SyncTestEntry2>();
            this.CascadedEntry2 = new SyncCascade<SyncTestEntry2>();
            this.CascadedReadOnlyEntry = new SyncCascadeReadOnly<SyncTestEntry2>();

            this.SimpleCollection = new SyncList<List<int>, int>();
            this.CascadingCollection = new SyncList<List<SyncTestEntry2>, SyncTestEntry2>();

            this.SimpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            this.CascadingDictionary = new SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SyncNull<int?> Id { get; private set; }

        public Sync<int> TestInt { get; set; }

        public Sync<long> TestLong { get; set; }

        public Sync<float> TestFloat { get; set; }

        public Sync<bool> TestBool { get; set; }

        public SyncObject<byte[]> ByteArray { get; private set; }

        public SyncObject<string> TestString { get; private set; }

        public Sync<TestEnum> Enum { get; set; }

        public SyncCascade<SyncTestEntry2> CascadedEntry { get; private set; }

        public SyncCascade<SyncTestEntry2> CascadedEntry2 { get; private set; }

        public SyncCascadeReadOnly<SyncTestEntry2> CascadedReadOnlyEntry { get; private set; }

        public SyncList<List<int>, int> SimpleCollection { get; private set; }

        public SyncList<List<SyncTestEntry2>, SyncTestEntry2> CascadingCollection { get; private set; }

        public SyncDictionary<Dictionary<string, float>, string, float> SimpleDictionary { get; private set; }

        public SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2> CascadingDictionary { get; private set; }

        public override bool IsChanged
        {
            get 
            {
                return this.Id.IsChanged
                    || this.TestInt.IsChanged
                    || this.TestLong.IsChanged
                    || this.TestFloat.IsChanged
                    || this.TestBool.IsChanged
                    || this.ByteArray.IsChanged
                    || this.TestString.IsChanged
                    || this.Enum.IsChanged
                    || this.CascadedEntry.IsChanged
                    || this.CascadedReadOnlyEntry.IsChanged
                    || this.SimpleCollection.IsChanged
                    || this.CascadingCollection.IsChanged
                    || this.SimpleDictionary.IsChanged
                    || this.CascadingDictionary.IsChanged;
            }
        }

        public override void Save(Stream target, bool ignoreChangeState = false)
        {
            // Simple types
            NativeSerialization.SerializeObject(target, ignoreChangeState || this.Id.IsChanged, this.Id.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestInt.IsChanged, this.TestInt.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestLong.IsChanged, this.TestLong.Value, Int64Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestFloat.IsChanged, this.TestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestBool.IsChanged, this.TestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.ByteArray.IsChanged, this.ByteArray.Value, ByteArraySerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestString.IsChanged, this.TestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.Enum.IsChanged, this.Enum.Value, (stream, value) => Int32Serializer.Instance.Serialize(stream, (int)value));

            // Cascaded objects
            NativeSerialization.SerializeCascade(target, this.CascadedEntry, ignoreChangeState);
            NativeSerialization.SerializeCascade(target, this.CascadedEntry2, ignoreChangeState);
            NativeSerialization.SerializeCascadeReadOnly(target, this.CascadedReadOnlyEntry, ignoreChangeState);

            // Lists
            NativeSerialization.SerializeList(target, ignoreChangeState || this.SimpleCollection.IsChanged, this.SimpleCollection.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.SerializeList(target, ignoreChangeState || this.CascadingCollection.IsChanged, this.CascadingCollection.Value, (stream, value) => value.Save(stream, true));
            
            // Dictionaries
            NativeSerialization.SerializeDictionary(target, ignoreChangeState || this.SimpleDictionary.IsChanged, this.SimpleDictionary.Value, StringSerializer.Instance.Serialize, FloatSerializer.Instance.Serialize);
            NativeSerialization.SerializeDictionary(target, ignoreChangeState || this.CascadingDictionary.IsChanged, this.CascadingDictionary.Value, Int32Serializer.Instance.Serialize, (stream, value) => value.Save(stream, true));
        }

        public override void Load(Stream source)
        {
            this.Id.Value = NativeSerialization.Deserialize(source, this.Id.Value, Int32Serializer.Instance.Deserialize);
            this.TestInt.Value = NativeSerialization.Deserialize(source, this.TestInt.Value, Int32Serializer.Instance.Deserialize);
            this.TestLong.Value = NativeSerialization.Deserialize(source, this.TestLong.Value, Int64Serializer.Instance.Deserialize);
            this.TestFloat.Value = NativeSerialization.Deserialize(source, this.TestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.TestBool.Value = NativeSerialization.Deserialize(source, this.TestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.ByteArray.Value = NativeSerialization.Deserialize(source, this.ByteArray.Value, ByteArraySerializer.Instance.Deserialize);
            this.TestString.Value = NativeSerialization.Deserialize(source, this.TestString.Value, StringSerializer.Instance.Deserialize);
            this.Enum.Value = NativeSerialization.Deserialize(source, this.Enum.Value, Int32Serializer.Instance.Deserialize);

            this.CascadedEntry.Value = NativeSerialization.DeserializeCascade(
                source,
                this.CascadedEntry.Value,
                () => new SyncTestEntry2());

            this.CascadedEntry2.Value = NativeSerialization.DeserializeCascade(
                source,
                this.CascadedEntry2.Value,
                () => new SyncTestEntry2());

            NativeSerialization.DeserializeCascadeReadOnly(source, this.CascadedReadOnlyEntry.Value);

            NativeSerialization.DeserializeList(
                    source,
                    this.SimpleCollection.Value,
                    Int32Serializer.Instance.Deserialize);

            NativeSerialization.DeserializeList(
                    source,
                    this.CascadingCollection.Value,
                    stream =>
                    {
                        var entry = new SyncTestEntry2();
                        entry.Load(stream);
                        return entry;
                    });

            NativeSerialization.DeserializeDictionary(
                source,
                this.SimpleDictionary.Value,
                StringSerializer.Instance.Deserialize,
                FloatSerializer.Instance.Deserialize);

            NativeSerialization.DeserializeDictionary(
                source,
                this.CascadingDictionary.Value,
                Int32Serializer.Instance.Deserialize,
                stream =>
                    {
                        var entry = new SyncTestEntry2();
                        entry.Load(stream);
                        return entry;
                    });
        }

        public override void ResetChangeState(bool state = false)
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
            this.CascadedEntry2.ResetChangeState(state);
            this.CascadedReadOnlyEntry.ResetChangeState(state);

            this.SimpleCollection.ResetChangeState(state);
            this.CascadingCollection.ResetChangeState(state);
            if (this.CascadingCollection.Value != null)
            {
                foreach (SyncTestEntry2 entry in this.CascadingCollection)
                {
                    entry.ResetChangeState(state);
                }
            }

            this.SimpleDictionary.ResetChangeState(state);
            this.CascadingDictionary.ResetChangeState(state);
            if (this.CascadingDictionary.Value != null)
            {
                foreach (int key in this.CascadingDictionary.Keys)
                {
                    this.CascadingDictionary[key].ResetChangeState(state);
                }
            }
        }
    }
}
