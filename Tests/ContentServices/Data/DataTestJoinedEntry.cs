namespace CarbonCore.Tests.ContentServices.Data
{
    using CarbonCore.ContentServices.Sql.Logic;
    using CarbonCore.ContentServices.Sql.Logic.Attributes;

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
            return this.Id == null ? 0: this.Id.GetHashCode() 
                    ^ this.TestEntryId.GetHashCode() 
                    ^ this.TestString.GetHashCode();
        }
    }
}
