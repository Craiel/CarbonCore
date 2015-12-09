namespace CarbonCore.Tests.Unity.Data
{
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class DataTestEntry : DataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DataElement(IgnoreClone = true)]
        public int? Id { get; set; }

        [DataElement]
        public string TestString { get; set; }

        [DataElement]
        public bool TestBool { get; set; }

        [DataElement]
        public float TestFloat { get; set; }

        [DataElement]
        public long TestLong { get; set; }

        [DataElement(IgnoreEquality = true)]
        public byte[] TestByteArray { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            int idHash = this.Id == null ? 0 : this.Id.GetHashCode();
            int stringHash = this.TestString == null ? 0 : this.TestString.GetHashCode();
            return idHash
                ^ stringHash
                ^ this.TestBool.GetHashCode()
                ^ this.TestFloat.GetHashCode()
                ^ this.TestLong.GetHashCode();
        }
    }
}
