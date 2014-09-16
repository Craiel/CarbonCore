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

        public bool Load(string hash, out byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            if (this.DoLoad(hash, out data))
            {
                this.BytesRead += data.Length;
                return true;
            }

            return false;
        }

        public bool Save(string hash, byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            if (this.DoSave(hash, data))
            {
                this.BytesWritten += data.Length;
                return true;
            }

            return false;
        }

        public bool Delete(string hash)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            if (this.DoDelete(hash))
            {
                this.EntriesDeleted++;
                return true;
            }

            return false;
        }

        public int Cleanup()
        {
            return this.DoCleanup();
        }

        public void Initialize()
        {
            System.Diagnostics.Trace.Assert(!this.IsInitialized);

            this.IsInitialized = this.DoInitialize();
        }

        public IList<IFileEntry> GetFiles()
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            return this.DoGetFiles();
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

        protected abstract bool DoInitialize();

        protected abstract bool DoLoad(string hash, out byte[] data);

        protected abstract bool DoSave(string hash, byte[] data);

        protected abstract bool DoDelete(string hash);

        protected abstract int DoCleanup();

        protected abstract IList<IFileEntry> DoGetFiles();
    }
}
