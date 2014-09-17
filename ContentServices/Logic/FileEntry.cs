namespace CarbonCore.ContentServices.Logic
{
    using System;
    
    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("FileTable")]
    public class FileEntry : DatabaseEntry
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
        public int Version { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public bool IsDeleted { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public long Size { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public DateTime CreateDate { get; set; }

        [DatabaseEntryElement]
        [ContentEntryElement(IgnoreEquality = true)]
        public DateTime ModifyDate { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Hash).GetHashCode();
        }
    }
}
