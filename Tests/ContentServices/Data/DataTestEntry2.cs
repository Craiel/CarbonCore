namespace CarbonCore.Tests.ContentServices.Data
{
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("TestTable2")]
    public class DataTestEntry2 : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Assigned, IgnoreClone = true)]
        public string Id { get; set; }

        [DatabaseEntryElement]
        public string OtherTestString { get; set; }

        [DatabaseEntryElement]
        public bool OtherTestBool { get; set; }

        [DatabaseEntryElement]
        public float OtherTestFloat { get; set; }

        [DatabaseEntryElement]
        public long OtherTestLong { get; set; }

#if UNITY
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
#endif
    }
}
