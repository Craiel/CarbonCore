namespace CarbonCore.ContentServices.Logic
{
    using System;
    
    using CarbonCore.ContentServices.Logic.Attributes;

    [DatabaseEntry("FileTable")]
    public class FileEntry : DatabaseEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileEntry()
        {
            // Initialize some sensible defaults
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.Version = 1;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement(PrimaryKeyMode = PrimaryKeyMode.Autoincrement, IgnoreClone = true, IgnoreEquality = true)]
        public int? Id { get; set; }

        [DatabaseEntryElement]
        public string Hash { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public int Version { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public bool IsDeleted { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public long Size { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
        public DateTime CreateDate { get; set; }

        [DatabaseEntryElement(IgnoreEquality = true)]
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
