namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public abstract class FileServiceProvider : IFileServiceProvider
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsInitialized { get; protected set; }

        public long Capacity { get; protected set; }

        public long Used { get; protected set; }

        public long BytesRead { get; private set; }

        public long BytesWritten { get; private set; }

        public long EntriesDeleted { get; private set; }

        public void Load(FileEntryKey key, out byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            this.DoLoad(key, out data);
            this.BytesRead += data.Length;
        }

        public void Save(FileEntryKey key, byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            this.DoSave(key, data);
            this.BytesWritten += data.Length;
        }

        public void Delete(FileEntryKey key)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);
            this.DoDelete(key);
            this.EntriesDeleted++;
        }

        public int Cleanup()
        {
            return this.DoCleanup();
        }

        public void Initialize()
        {
            System.Diagnostics.Trace.Assert(!this.IsInitialized);
            this.DoInitialize();
            this.IsInitialized = true;
        }

        public IList<FileEntryKey> GetFiles(bool includeDeleted = false)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            return this.DoGetFiles(includeDeleted);
        }

        public void SetVersion(FileEntryKey key, int version)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.Version = version;
            this.SaveEntry(key, entry);
        }

        public int GetVersion(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.Version;
        }

        public void SetCreateDate(FileEntryKey key, DateTime date)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.CreateDate = date;
            this.SaveEntry(key, entry);
        }

        public DateTime GetCreateDate(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.CreateDate;
        }

        public void SetModifiedDate(FileEntryKey key, DateTime date)
        {
            FileEntry entry = this.LoadEntry(key);
            entry.ModifyDate = date;
            this.SaveEntry(key, entry);
        }

        public DateTime GetModifiedDate(FileEntryKey key)
        {
            FileEntry entry = this.LoadEntry(key);
            return entry.ModifyDate;
        }

        public void GetMetadata(FileEntryKey key, int metadataKey, out int? value, out string stringValue)
        {
            throw new NotImplementedException();
        }

        public void SetMetadata(FileEntryKey key, int metadataKey, int? value = null, string stringValue = null)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
        }

        protected abstract void DoInitialize();

        protected abstract void DoLoad(FileEntryKey key, out byte[] data);

        protected abstract void DoSave(FileEntryKey key, byte[] data);

        protected abstract void DoDelete(FileEntryKey key);

        protected abstract int DoCleanup();

        protected abstract FileEntry LoadEntry(FileEntryKey key);
        protected abstract void SaveEntry(FileEntryKey key, FileEntry entry);

        protected abstract IList<FileEntryKey> DoGetFiles(bool includeDeleted);
    }
}
