namespace CarbonCore.Tests.ContentServices
{
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("TestTable")]
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
    }
}
