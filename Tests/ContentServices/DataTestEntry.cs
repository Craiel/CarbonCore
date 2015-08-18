﻿namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat.Json;

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
            var loadedEntry = JsonExtensions.LoadFromStream<DataTestEntry>(source);
            this.CopyFrom(loadedEntry);

            return loadedEntry != null;
        }

        public override bool Save(System.IO.Stream target)
        {
            JsonExtensions.SaveToStream(target, this);
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
