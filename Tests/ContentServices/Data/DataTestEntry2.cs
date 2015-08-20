namespace CarbonCore.Tests.ContentServices
{
    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.ContentServices.Compat.Logic.Attributes;

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
    }
}
