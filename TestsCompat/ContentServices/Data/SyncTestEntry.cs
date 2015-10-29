namespace CarbonCore.Tests.Compat.ContentServices.Data
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
            this.CascadingCollection = new SyncCascadeList<List<SyncTestEntry2>, SyncTestEntry2>();

            this.SimpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            this.EnumDictionary = new SyncDictionary<Dictionary<TestEnum, double>, TestEnum, double>();
            this.CascadingDictionary = new SyncCascadeValueDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
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

        public SyncCascadeList<List<SyncTestEntry2>, SyncTestEntry2> CascadingCollection { get; private set; }

        public SyncDictionary<Dictionary<string, float>, string, float> SimpleDictionary { get; private set; }

        public SyncDictionary<Dictionary<TestEnum, double>, TestEnum, double> EnumDictionary { get; private set; }

        public SyncCascadeValueDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2> CascadingDictionary { get; private set; }

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
                    || this.EnumDictionary.IsChanged
                    || this.CascadingDictionary.IsChanged;
            }
        }

        public override void Save(Stream target, bool ignoreChangeState = false)
        {
            // Simple types
            NativeSerialization.SerializeObject(target, ignoreChangeState || this.Id.IsChanged, this.Id.Value, Int32Serializer.Instance.SerializeNullable);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestInt.IsChanged, this.TestInt.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestLong.IsChanged, this.TestLong.Value, Int64Serializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestFloat.IsChanged, this.TestFloat.Value, FloatSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestBool.IsChanged, this.TestBool.Value, BooleanSerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.ByteArray.IsChanged, this.ByteArray.Value, ByteArraySerializer.Instance.Serialize);
            NativeSerialization.Serialize(target, ignoreChangeState || this.TestString.IsChanged, this.TestString.Value, StringSerializer.Instance.Serialize);
            NativeSerialization.SerializeEnum(target, ignoreChangeState || this.Enum.IsChanged, this.Enum.Value);

            // Cascaded objects
            NativeSerialization.SerializeCascade(target, this.CascadedEntry, ignoreChangeState);
            NativeSerialization.SerializeCascade(target, this.CascadedEntry2, ignoreChangeState);
            NativeSerialization.SerializeCascadeReadOnly(target, this.CascadedReadOnlyEntry, ignoreChangeState);

            // Lists
            NativeSerialization.SerializeList(target, ignoreChangeState || this.SimpleCollection.IsChanged, this.SimpleCollection.Value, Int32Serializer.Instance.Serialize);
            NativeSerialization.SerializeCascadeList(target, this.CascadingCollection, ignoreChangeState);
            
            // Dictionaries
            NativeSerialization.SerializeDictionary(target, ignoreChangeState || this.SimpleDictionary.IsChanged, this.SimpleDictionary.Value, StringSerializer.Instance.Serialize, FloatSerializer.Instance.Serialize);

            NativeSerialization.SerializeDictionary(
                target,
                ignoreChangeState || this.EnumDictionary.IsChanged,
                this.EnumDictionary.Value,
                (stream, value) => Int32Serializer.Instance.Serialize(stream, (int)value),
                DoubleSerializer.Instance.Serialize);

            NativeSerialization.SerializeCascadeValueDictionary(target, this.CascadingDictionary, Int32Serializer.Instance.Serialize, ignoreChangeState);
        }

        public override void Load(Stream source)
        {
            this.Id.Value = NativeSerialization.Deserialize(source, this.Id.Value, Int32Serializer.Instance.DeserializeNullable);
            this.TestInt.Value = NativeSerialization.Deserialize(source, this.TestInt.Value, Int32Serializer.Instance.Deserialize);
            this.TestLong.Value = NativeSerialization.Deserialize(source, this.TestLong.Value, Int64Serializer.Instance.Deserialize);
            this.TestFloat.Value = NativeSerialization.Deserialize(source, this.TestFloat.Value, FloatSerializer.Instance.Deserialize);
            this.TestBool.Value = NativeSerialization.Deserialize(source, this.TestBool.Value, BooleanSerializer.Instance.Deserialize);
            this.ByteArray.Value = NativeSerialization.Deserialize(source, this.ByteArray.Value, ByteArraySerializer.Instance.Deserialize);
            this.TestString.Value = NativeSerialization.Deserialize(source, this.TestString.Value, StringSerializer.Instance.Deserialize);
            this.Enum.Value = NativeSerialization.DeserializeEnum(source, this.Enum.Value, value => (TestEnum)value);

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

            NativeSerialization.DeserializeCascadeList(
                    source,
                    this.CascadingCollection,
                    () => new SyncTestEntry2());

            NativeSerialization.DeserializeDictionary(
                source,
                this.SimpleDictionary.Value,
                StringSerializer.Instance.Deserialize,
                FloatSerializer.Instance.Deserialize);

            NativeSerialization.DeserializeDictionary(
                source,
                this.EnumDictionary.Value,
                stream => (TestEnum)Int32Serializer.Instance.Deserialize(stream),
                DoubleSerializer.Instance.Deserialize);

            NativeSerialization.DeserializeCascadeValueDictionary(
                source,
                this.CascadingDictionary,
                Int32Serializer.Instance.Deserialize,
                () => new SyncTestEntry2());
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

            this.SimpleDictionary.ResetChangeState(state);
            this.EnumDictionary.ResetChangeState(state);
            this.CascadingDictionary.ResetChangeState(state);
        }
    }
}
