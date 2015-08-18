namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

    using Newtonsoft.Json;

    [DatabaseEntry("TestTable")]
    [JsonObject(MemberSerialization.OptOut)]
    public class DataTestEntry : DatabaseEntry
    {
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement, IgnoreClone = true)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public int TestInt { get; set; }

        [DatabaseEntryElement]
        public long TestLong { get; set; }

        [DatabaseEntryElement]
        public float TestFloat { get; set; }

        [DatabaseEntryElement]
        public bool TestBool { get; set; }

        [DatabaseEntryElement]
        public byte[] ByteArray { get; set; }

        [DatabaseEntryElement]
        public string TestString { get; set; }

        [DataElement]
        public DataTestEntry2 CascadedEntry { get; set; }

        [DataElement]
        public IList<int> SimpleCollection { get; set; }

        [DataElement]
        public IList<DataTestEntry2> CascadingCollection { get; set; }

        [DataElement]
        public IDictionary<string, float> SimpleDictionary { get; set; }

        [DataElement]
        public IDictionary<int, DataTestEntry2> CascadingDictionary { get; set; }

        [DatabaseEntryJoinedElement("TestEntryId", IgnoreEquality = true)]
        public DataTestJoinedEntry JoinedEntry { get; set; }

        public override bool Load(System.IO.Stream source)
        {
            this.TestInt = (int)Int32Serializer.Instance.Deserialize(source);
            this.TestLong = (long)Int64Serializer.Instance.Deserialize(source);
            this.TestFloat = (float)FloatSerializer.Instance.Deserialize(source);
            this.TestBool = (bool)BooleanSerializer.Instance.Deserialize(source);
            this.ByteArray = (byte[])ByteArraySerializer.Instance.Deserialize(source);
            this.TestString = (string)StringSerializer.Instance.Deserialize(source);

            return true;
        }

        public override bool Save(System.IO.Stream target)
        {
            Int32Serializer.Instance.Serialize(target, this.TestInt);
            Int64Serializer.Instance.Serialize(target, this.TestLong);
            FloatSerializer.Instance.Serialize(target, this.TestFloat);
            BooleanSerializer.Instance.Serialize(target, this.TestBool);
            ByteArraySerializer.Instance.Serialize(target, this.ByteArray);
            StringSerializer.Instance.Serialize(target, this.TestString);

            return true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Id).GetHashCode();
        }
    }
}
