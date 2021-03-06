﻿namespace CarbonCore.Tests.ContentServices.Data
{
    using System.Collections.Generic;
    
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Sql.Logic;
    using CarbonCore.ContentServices.Sql.Logic.Attributes;

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

        [DatabaseEntryElement(IgnoreEquality = true)]
        public byte[] ByteArray { get; set; }

        [DatabaseEntryElement]
        public string TestString { get; set; }

        [DatabaseEntryElement]
        public TestEnum Enum { get; set; }

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
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return this.Id == null ? 0 : this.Id.GetHashCode();
        }
    }
}
