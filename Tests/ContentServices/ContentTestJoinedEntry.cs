namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;

    using Newtonsoft.Json;

    [DatabaseEntry("TestJoinedTable")]
    [JsonObject(MemberSerialization.OptOut)]
    public class ContentTestJoinedEntry : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(IgnoreClone = true)]
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement)]
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
