namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils;

    public class FileService : IFileService
    {
        private readonly IList<IFileServiceProvider> providers;
        private readonly IList<IFileEntry> fileEntryLookup; 
        private readonly IDictionary<IFileEntry, IFileServiceProvider> fileProviderLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileService()
        {
            this.providers = new List<IFileServiceProvider>();
            this.fileEntryLookup = new List<IFileEntry>();
            this.fileProviderLookup = new Dictionary<IFileEntry, IFileServiceProvider>();
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

        public IFileEntryData Load(IFileEntry key)
        {
            throw new NotImplementedException();
        }

        public bool Save(IFileEntry key, IFileEntryData data, string internalFileName = null, IFileServiceProvider targetProvider = null)
        {
            if ((key.Hash == null && string.IsNullOrEmpty(internalFileName))
                || (!string.IsNullOrEmpty(key.Hash) && !string.IsNullOrEmpty(internalFileName)))
            {
                throw new ArgumentException("Can not have key and internal file name specified at once (or omitted)");
            }

            if (string.IsNullOrEmpty(key.Hash))
            {
                System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(internalFileName));
                key.Hash = HashUtils.GetSHA1FileName(internalFileName);
            }

            // Save to a specific provider or into the first available one...
            if (targetProvider == null)
            {
                // Use the provider this is already in before trying to pick!
                if (this.fileProviderLookup.ContainsKey(key))
                {
                    if (this.fileProviderLookup[key].Capacity > data.Data.Length)
                    {
                        targetProvider = this.fileProviderLookup[key];
                    }
                }
                else
                {
                    foreach (IFileServiceProvider provider in this.providers)
                    {
                        if (provider.Capacity > data.Data.Length)
                        {
                            targetProvider = provider;
                            break;
                        }
                    }    
                }
            }
            else
            {
                System.Diagnostics.Trace.Assert(this.providers.Contains(targetProvider), "Target provider must be registered");
            }

            if (targetProvider == null)
            {
                throw new InvalidOperationException("Could not determine operator with enough capacity");
            }

            if (targetProvider.Save(key, data.Data))
            {
                this.fileEntryLookup.Add(key);
                this.fileProviderLookup.Add(key, targetProvider);
                return true;
            }

            return false;
        }
        
        public bool Delete(IFileEntry key)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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

        public bool CheckForUpdate(IFileEntry key)
        {
            throw new NotImplementedException();
        }

        public IList<IFileEntry> GetFileEntries()
        {
            return new List<IFileEntry>(this.fileEntryLookup);
        }

        public IList<IFileServiceProvider> GetProviders()
        {
            return new List<IFileServiceProvider>(this.providers);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            IList<IFileServiceProvider> providerList = new List<IFileServiceProvider>(this.providers);
            foreach (IFileServiceProvider provider in providerList)
            {
                this.MixOutProvider(provider);
            }

            this.providers.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void MixInProvider(IFileServiceProvider provider)
        {
            this.providers.Add(provider);

            IList<IFileEntry> files = provider.GetFiles();
            foreach (IFileEntry entry in files)
            {
                int existingIndex = this.fileEntryLookup.IndexOf(entry);
                if (existingIndex >= 0)
                {
                    IFileEntry existing = this.fileEntryLookup[existingIndex];
                    if (existing.Version >= entry.Version)
                    {
                        System.Diagnostics.Trace.TraceWarning("Ignoring entry {0} in provider {1}, already present", entry, provider);
                        continue;
                    }

                    System.Diagnostics.Trace.TraceWarning("Changing instance of {0} to newer version {1} -> {2}", entry, existing.Version, entry.Version);
                    this.fileEntryLookup.RemoveAt(existingIndex);
                    this.fileProviderLookup.Remove(existing);
                }

                this.fileEntryLookup.Add(entry);
                this.fileProviderLookup.Add(entry, provider);
            }
        }

        private void MixOutProvider(IFileServiceProvider provider)
        {
            System.Diagnostics.Trace.Assert(this.providers.Contains(provider));

            foreach (IFileEntry file in provider.GetFiles())
            {
                // Check if this provider's file / version is what we are using
                if (this.fileProviderLookup.ContainsKey(file) && this.fileProviderLookup[file] == provider)
                {
                    // If so, Mix out that file
                    this.fileEntryLookup.Remove(file);
                    this.fileProviderLookup.Remove(file);
                }
            }

            this.providers.Remove(provider);
        }
    }
}
