namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils;

    public class FileService : IFileService
    {
        private readonly IList<IFileServiceProvider> providers;
        private readonly IList<FileEntry> fileEntryLookup; 
        private readonly IDictionary<FileEntry, IFileServiceProvider> fileProviderLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileService()
        {
            this.providers = new List<IFileServiceProvider>();
            this.fileEntryLookup = new List<FileEntry>();
            this.fileProviderLookup = new Dictionary<FileEntry, IFileServiceProvider>();
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

        public IFileEntryData Load(FileEntry key)
        {
            throw new NotImplementedException();
        }

        public bool Save(FileEntry key, IFileEntryData data, string internalFileName = null, IFileServiceProvider targetProvider = null)
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
            // Use the provider this is already in before trying to pick!
            if (this.fileProviderLookup.ContainsKey(key))
            {
                if (this.fileProviderLookup[key].Capacity > data.Data.Length)
                {
                    targetProvider = this.fileProviderLookup[key];
                }
            }
            else if (targetProvider == null)
            {
                throw new InvalidOperationException("Target provider must be supplied or the file must already be present");
            }

            if (targetProvider == null)
            {
                throw new InvalidOperationException("Could not determine operator with enough capacity");
            }

            if (targetProvider.Save(key.Hash, data.Data))
            {
                if (this.fileProviderLookup.ContainsKey(key))
                {
                    this.fileProviderLookup[key] = targetProvider;
                }
                else
                {
                    this.fileEntryLookup.Add(key);
                    this.fileProviderLookup.Add(key, targetProvider);
                }
                
                return true;
            }

            return false;
        }
        
        public bool Delete(FileEntry key)
        {
            System.Diagnostics.Trace.Assert(this.fileProviderLookup.ContainsKey(key));

            if (this.fileProviderLookup[key].Delete(key.Hash))
            {
                this.fileEntryLookup.Remove(key);
                this.fileProviderLookup.Remove(key);
                return true;
            }

            return false;
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

        public bool CheckForUpdate(FileEntry key)
        {
            throw new NotImplementedException();
        }

        public IList<FileEntry> GetFileEntries()
        {
            return new List<FileEntry>(this.fileEntryLookup);
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

            IList<FileEntry> files = provider.GetFiles();
            if (files == null)
            {
                return;
            }

            foreach (FileEntry entry in files)
            {
                int existingIndex = this.fileEntryLookup.IndexOf(entry);
                if (existingIndex >= 0)
                {
                    FileEntry existing = this.fileEntryLookup[existingIndex];
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

            IList<FileEntry> files = provider.GetFiles();
            if (files != null)
            {
                foreach (FileEntry file in files)
                {
                    // Check if this provider's file / version is what we are using
                    if (this.fileProviderLookup.ContainsKey(file) && this.fileProviderLookup[file] == provider)
                    {
                        // If so, Mix out that file
                        this.fileEntryLookup.Remove(file);
                        this.fileProviderLookup.Remove(file);
                    }
                }
            }

            this.providers.Remove(provider);
        }
    }
}
