namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.ContentServices.Compat.Logic.Attributes;

    using Newtonsoft.Json;

    [DatabaseEntry("TestJoinedTable")]
    [JsonObject(MemberSerialization.OptOut)]
    public class DataTestJoinedEntry : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement, IgnoreClone = true)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public int TestEntryId { get; set; }

        [DatabaseEntryElement]
        public string TestString { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Id, this.TestEntryId).GetHashCode();
        }
    }
}
