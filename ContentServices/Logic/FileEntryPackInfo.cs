namespace CarbonCore.ContentServices.Logic
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("FileTablePackInfo")]
    public class FileEntryPackInfo : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement)]
        [ContentEntryElement(IgnoreClone = true, IgnoreEquality = true)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public string Hash { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public long Offset { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public long Size { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Hash).GetHashCode();
        }
    }
}
