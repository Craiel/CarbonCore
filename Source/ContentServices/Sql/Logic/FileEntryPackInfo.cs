namespace CarbonCore.ContentServices.Sql.Logic
{
    using CarbonCore.ContentServices.Sql.Logic.Attributes;

    [DatabaseEntry("FileTablePackInfo")]
    public class FileEntryPackInfo : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement, IgnoreClone = true, IgnoreEquality = true)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public string Hash { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public long Offset { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public long Size { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public long Padding { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return this.Hash.GetHashCode();
        }
    }
}
