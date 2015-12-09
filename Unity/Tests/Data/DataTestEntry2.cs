namespace CarbonCore.Tests.Unity.Data
{
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class DataTestEntry2 : DataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DataElement(IgnoreClone = true)]
        public string Id { get; set; }

        [DataElement]
        public string OtherTestString { get; set; }

        [DataElement]
        public bool OtherTestBool { get; set; }

        [DataElement]
        public float OtherTestFloat { get; set; }

        [DataElement]
        public long OtherTestLong { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            int idHash = this.Id == null ? 0 : this.Id.GetHashCode();
            int stringHash = this.OtherTestString == null ? 0 : this.OtherTestString.GetHashCode();
            return idHash
                ^ stringHash
                ^ this.OtherTestBool.GetHashCode()
                ^ this.OtherTestFloat.GetHashCode()
                ^ this.OtherTestLong.GetHashCode();
        }
    }
}
