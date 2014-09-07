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

        public bool Load(IFileEntry key, out byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            if (this.DoLoad(key, out data))
            {
                this.BytesRead += data.Length;
                return true;
            }

            return false;
        }

        public bool Save(IFileEntry key, byte[] data)
        {
            System.Diagnostics.Trace.Assert(this.IsInitialized);

            if (this.DoSave(key, data))
            {
                this.BytesWritten += data.Length;
                return true;
            }

            return false;
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

        protected abstract bool DoLoad(IFileEntry key, out byte[] data);

        protected abstract bool DoSave(IFileEntry key, byte[] data);

        protected abstract IList<IFileEntry> DoGetFiles();
    }
}
