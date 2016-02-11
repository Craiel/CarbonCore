namespace CarbonCore.CFS.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using CarbonCore.CFS.Contracts;
    using CarbonCore.Utils.IO;

    public abstract class CFSInstance : ICFSInstance
    {
        private readonly List<CarbonFile> files;
         
        private CFSFileTable primaryTable;
        private CFSFileTable backupTable;

        private Stream stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CFSInstance()
        {
            this.files = new List<CarbonFile>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsInitialized { get; private set; }

        public ReadOnlyCollection<CarbonFile> Files
        {
            get
            {
                return this.files.AsReadOnly();
            }
        }

        public Stream BeginRead(CarbonFile file)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(CarbonFile file, out byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public bool Read(CarbonFile file, out string data)
        {
            throw new System.NotImplementedException();
        }

        public void Store(CarbonFile file)
        {
            throw new System.NotImplementedException();
        }

        public void Store(CarbonFile file, Stream data)
        {
            throw new System.NotImplementedException();
        }

        public void Store(CarbonFile file, byte[] data)
        {
            throw new System.NotImplementedException();
        }

        public void Store(CarbonFile file, string data)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(CarbonFile file)
        {
            throw new System.NotImplementedException();
        }

        public IList<CarbonFile> Find(ICFSFilter filter)
        {
            throw new System.NotImplementedException();
        }

        public void SetMetaData(CarbonFile file, int key, object value)
        {
            throw new System.NotImplementedException();
        }

        public void SetMetaData(CarbonFile file, IDictionary<int, object> data)
        {
            throw new System.NotImplementedException();
        }

        public object GetMetaData(CarbonFile file, int key)
        {
            throw new System.NotImplementedException();
        }

        public IDictionary<int, object> GetMetaData(CarbonFile file)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Initialize(Stream newStream)
        {
            this.stream = newStream;

            this.IsInitialized = true;
        }

        protected virtual void Dispose(bool isDisposing)
        {
            
        }
    }
}
