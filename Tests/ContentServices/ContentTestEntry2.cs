namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("TestTable2")]
    public class ContentTestEntry2 : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement(IgnoreClone = true)]
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Assigned)]
        public string Id { get; set; }

        [DatabaseEntryElement]
        public string OtherTestString { get; set; }

        [DatabaseEntryElement]
        public bool OtherTestBool { get; set; }

        [DatabaseEntryElement]
        public float OtherTestFloat { get; set; }

        [DatabaseEntryElement]
        public long OtherTestLong { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Id).GetHashCode();
        }
    }
}
