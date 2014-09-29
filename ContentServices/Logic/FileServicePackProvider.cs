namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public class FileServicePackProvider : FileServiceProvider, IFileServicePackProvider
    {
        private readonly IDictionary<FileEntryKey, long> files;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServicePackProvider()
        {
            this.files = new Dictionary<FileEntryKey, long>();
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoInitialize()
        {
            // Todo
        }

        protected override void DoLoad(FileEntryKey key, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override void DoSave(FileEntryKey key, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        protected override void DoDelete(FileEntryKey key)
        {
            throw new System.NotImplementedException();
        }

        protected override int DoCleanup()
        {
            throw new System.NotImplementedException();
        }

        protected override FileEntry LoadEntry(FileEntryKey key)
        {
            throw new System.NotImplementedException();
        }

        protected override void SaveEntry(FileEntryKey key, FileEntry entry)
        {
            throw new System.NotImplementedException();
        }
        
        protected override IList<FileEntryKey> DoGetFiles(bool includeDeleted)
        {
            return new List<FileEntryKey>(this.files.Keys);
        }
    }
}
