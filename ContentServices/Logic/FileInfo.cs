namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;

    public class FileInfo : IFileInfo
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ContentEntryElement]
        public string Hash { get; set; }

        [ContentEntryElement(IgnoreEquality = true)]
        public int Version { get; private set; }

        [ContentEntryElement(IgnoreEquality = true)]
        public long Size { get; private set; }

        [ContentEntryElement(IgnoreEquality = true)]
        public DateTime CreateDate { get; private set; }

        [ContentEntryElement(IgnoreEquality = true)]
        public DateTime ModifyDate { get; private set; }
        
        public void Update(int newVersion, DateTime createDate, DateTime modifyDate, long size)
        {
            this.Version = newVersion;
            this.CreateDate = createDate;
            this.ModifyDate = modifyDate;
            this.Size = size;
        }

        public IContentEntry Clone()
        {
            var clone = new FileInfo { Hash = this.Hash };
            clone.Update(this.Version, this.CreateDate, this.ModifyDate, this.Size);
            return clone;
        }

        public bool Load(Stream source)
        {
            throw new NotImplementedException();
        }

        public bool Save(Stream target)
        {
            throw new NotImplementedException();
        }
    }
}
