﻿namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [DatabaseEntry("TestTable")]
    [JsonObject(MemberSerialization.OptOut)]
    public class ContentTestEntry : DatabaseEntry
    {
        [ContentEntryElement(IgnoreClone = true)]
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public string TestString { get; set; }

        [DatabaseEntryElement]
        public bool TestBool { get; set; }

        [DatabaseEntryElement]
        public float TestFloat { get; set; }

        [DatabaseEntryElement]
        public long TestLong { get; set; }

        [ContentEntryElement(IgnoreEquality = true)]
        [DatabaseEntryElement]
        public byte[] TestByteArray { get; set; }

        public override bool Load(System.IO.Stream source)
        {
            var loadedEntry = JsonExtensions.LoadFromStream<ContentTestEntry>(source);
            this.CopyFrom(loadedEntry);

            return loadedEntry != null;
        }

        public override bool Save(System.IO.Stream target)
        {
            JsonExtensions.SaveToStream(target, this);
            return true;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Id).GetHashCode();
        }
    }
}
