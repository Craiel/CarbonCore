namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;

    public class FileService : IFileService
    {
        private readonly IList<IFileServiceProvider> providers;
        private IDictionary<IFileInfo, IFileServiceProvider> fileProviderLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileService()
        {
            this.providers = new List<IFileServiceProvider>();
            this.fileProviderLookup = new Dictionary<IFileInfo, IFileServiceProvider>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int FileCount
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T Load<T>(IFileInfo key) where T : IFileEntry
        {
            throw new NotImplementedException();
        }

        public bool Save(IFileInfo key, IFileEntry source)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IFileInfo key)
        {
            throw new NotImplementedException();
        }

        public bool Reload(IFileInfo key, IFileEntry target)
        {
            throw new NotImplementedException();
        }

        public void Release(IFileInfo key, ref IFileEntry data)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
        
        public void AddProvider(IFileServiceProvider provider)
        {
            System.Diagnostics.Trace.Assert(!this.providers.Contains(provider));

            this.MixInProvider(provider);
        }

        public void RemoveProvider(IFileServiceProvider provider)
        {
            System.Diagnostics.Trace.Assert(this.providers.Contains(provider));

            this.MixOutProvider(provider);
        }

        public bool CheckForUpdate(IFileInfo key)
        {
            throw new NotImplementedException();
        }
        
        public IList<IFileInfo> GetFileInfos()
        {
            throw new System.NotImplementedException();
        }

        public IList<IFileServiceProvider> GetProviders()
        {
            return new List<IFileServiceProvider>(this.providers);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void MixInProvider(IFileServiceProvider provider)
        {
            throw new NotImplementedException();
        }

        private void MixOutProvider(IFileServiceProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
