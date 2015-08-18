namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using Newtonsoft.Json;

    [DatabaseEntry("TestTable")]
    [JsonObject(MemberSerialization.OptOut)]
    public class SyncTestEntryCombined : DatabaseEntry
    {
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement, IgnoreClone = true)]
        public Sync<int?> Id { get; set; }

        [DatabaseEntryElement]
        public Sync<int> TestInt { get; set; }

        [DatabaseEntryElement]
        public Sync<long> TestLong { get; set; }

        [DatabaseEntryElement]
        public Sync<float> TestFloat { get; set; }

        [DatabaseEntryElement]
        public Sync<bool> TestBool { get; set; }

        [DatabaseEntryElement]
        public Sync<byte[]> ByteArray { get; set; }

        [DatabaseEntryElement]
        public Sync<string> TestString { get; set; }

        [DataElement]
        public Sync<SyncTestEntryCombined2> CascadedEntry { get; set; }

        [DataElement]
        public Sync<IList<int>> SimpleCollection { get; set; }

        [DataElement]
        public Sync<IList<SyncTestEntryCombined2>> CascadingCollection { get; set; }

        [DataElement]
        public Sync<IDictionary<string, float>> SimpleDictionary { get; set; }

        [DataElement]
        public Sync<IDictionary<int, SyncTestEntryCombined2>> CascadingDictionary { get; set; }
        
        public override int NativeSave(Stream target)
        {
            Serialize(target, this.Id.IsChanged, this.Id.Value);
            return base.NativeSave(target);
        }

        public override void NativeLoad(Stream source)
        {
            base.NativeLoad(source);
        }

        private static int Serialize<T>(Stream stream, bool isChanged, object value)
        {
            if (isChanged)
            {
                stream.WriteByte(0);
                return 1;
            }

            stream.WriteByte(1);
            // Todo..

            return 1;
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
