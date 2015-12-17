namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Data;

    public class FileService : IFileService
    {
        private readonly IList<IFileServiceProvider> providers;
        private readonly IDictionary<FileEntryKey, IFileServiceProvider> fileProviderLookup;

        private IFileServiceProvider defaultProvider;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileService()
        {
            this.providers = new List<IFileServiceProvider>();
            this.fileProviderLookup = new Dictionary<FileEntryKey, IFileServiceProvider>();
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

        public IFileServiceProvider DefaultProvider
        {
            get
            {
                return this.defaultProvider;
            }

            set
            {
                if (value != null)
                {
                    if (!this.providers.Contains(value))
                    {
                        throw new InvalidOperationException("Default provider must be registered before setting!");
                    }
                }

                this.defaultProvider = value;
            }
        }
        
        public FileEntryData Load(FileEntryKey key)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            byte[] data;
            provider.Load(key, out data);
            return new FileEntryData(data);
        }

        public void Save(FileEntryKey key, FileEntryData data, IFileServiceProvider targetProvider = null)
        {
            targetProvider = targetProvider ?? this.defaultProvider;
            bool keyExists = this.fileProviderLookup.ContainsKey(key);

            if (targetProvider == null && !keyExists)
            {
                throw new InvalidOperationException("Target provider must be supplied or file must already exist");
            }

            if (keyExists)
            {
                this.fileProviderLookup[key] = targetProvider;
            }
            else
            {
                targetProvider.Save(key, data.ByteData);
                this.fileProviderLookup.Add(key, targetProvider);
            }
        }

        public void Update(FileEntryKey key, FileEntryData data, bool autoIncrementVersion = true)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new InvalidOperationException(string.Format("Can not update entry {0}, not yet present in the service", key));
            }

            provider.Save(key, data.ByteData);

            // Update modified
            provider.SetModifiedDate(key, DateTime.Now);

            // Update version if desired
            if (autoIncrementVersion)
            {
                int currentVersion = provider.GetVersion(key);
                this.SetVersion(key, currentVersion + 1);
            }
        }

        public void Delete(FileEntryKey key)
        {
            System.Diagnostics.Trace.Assert(this.fileProviderLookup.ContainsKey(key));

            this.fileProviderLookup[key].Delete(key);
            this.fileProviderLookup.Remove(key);
        }

        public void Move(FileEntryKey key, IFileServiceProvider targetProvider)
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

            // Make sure to remove this provider as the default if it was set to be
            if (this.defaultProvider == provider)
            {
                this.defaultProvider = null;
            }
        }
        
        public IList<FileEntryKey> GetFileEntries()
        {
            return new List<FileEntryKey>(this.fileProviderLookup.Keys);
        }

        public IFileServiceProvider GetProvider(FileEntryKey key)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            return provider;
        }

        public IList<IFileServiceProvider> GetProviders()
        {
            return new List<IFileServiceProvider>(this.providers);
        }

        public void SetVersion(FileEntryKey key, int version)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new InvalidDataException(string.Format("Entry {0} does not exist", key));
            }

            System.Diagnostics.Trace.TraceWarning("Warning! Changing version of {0} to {1}", key, version);
            provider.SetVersion(key, version);
        }

        public int GetVersion(FileEntryKey key)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new InvalidDataException(string.Format("Entry {0} does not exist", key));
            }

            return provider.GetVersion(key);
        }

        public void SetCreateDate(FileEntryKey key, DateTime date)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            System.Diagnostics.Trace.TraceWarning("Warning! Changing create date of {0} to {1}", key, date);
            provider.SetCreateDate(key, date);
        }

        public DateTime GetCreateDate(FileEntryKey key)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            return provider.GetCreateDate(key);
        }

        public void SetModifiedDate(FileEntryKey key, DateTime date)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            DateTime createDate = provider.GetCreateDate(key);
            if (date < createDate)
            {
                throw new ArgumentException("Modified date has to be equal or newer than Create date");
            }

            System.Diagnostics.Trace.TraceWarning("Warning! Changing modified date of {0} to {1}", key, date);
            provider.SetModifiedDate(key, date);
        }

        public DateTime GetModifiedDate(FileEntryKey key)
        {
            IFileServiceProvider provider;
            if (!this.fileProviderLookup.TryGetValue(key, out provider))
            {
                throw new KeyNotFoundException(string.Format("File does not exist: {0}", key));
            }

            return provider.GetModifiedDate(key);
        }

        public int Cleanup()
        {
            int results = 0;
            foreach (IFileServiceProvider provider in this.providers)
            {
                results += provider.Cleanup();
            }

            return results;
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

            IList<FileEntryKey> files = provider.GetFiles();
            if (files == null)
            {
                return;
            }

            foreach (FileEntryKey key in files)
            {
                // If we don't have this file yet just add it
                if (!this.fileProviderLookup.ContainsKey(key))
                {
                    this.fileProviderLookup.Add(key, provider);
                    continue;
                }

                // we already have this file so we need to compare versions to know which one to keep
                int currentVersion = this.fileProviderLookup[key].GetVersion(key);
                int providerVersion = provider.GetVersion(key);
                if (currentVersion <= providerVersion)
                {
                    System.Diagnostics.Trace.TraceWarning("Ignoring entry {0} in {1}, version {2} < {3} from {4}", key, provider, providerVersion, currentVersion, this.fileProviderLookup[key]);
                    continue;
                }

                // The provider version is more recent than the one we have so replace
                System.Diagnostics.Trace.TraceWarning("Replacing version {0} of {1} ({2}) with version {3} from {4}", currentVersion, key, this.fileProviderLookup[key], providerVersion, provider);
                this.fileProviderLookup[key] = provider;
            }
        }

        private void MixOutProvider(IFileServiceProvider provider)
        {
            System.Diagnostics.Trace.Assert(this.providers.Contains(provider));

            IList<FileEntryKey> files = provider.GetFiles();
            if (files != null)
            {
                foreach (FileEntryKey key in files)
                {
                    // Check if this provider's file / version is what we are using
                    IFileServiceProvider currentProvider;
                    if (this.fileProviderLookup.TryGetValue(key, out currentProvider) && currentProvider == provider)
                    {
                        // If so, Mix out that file
                        this.fileProviderLookup.Remove(key);
                    }
                }
            }

            this.providers.Remove(provider);
        }
    }
}
