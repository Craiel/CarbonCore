namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IFileService : IDisposable
    {
        int FileCount { get; }

        IFileEntryData Load(IFileEntry key);
        bool Save(IFileEntry key, IFileEntryData data, IFileServiceProvider targetProvider = null);
        bool Delete(IFileEntry key);

        void AddProvider(IFileServiceProvider provider);
        void RemoveProvider(IFileServiceProvider provider);

        bool CheckForUpdate(IFileEntry key);

        IList<IFileEntry> GetFileEntries();

        IList<IFileServiceProvider> GetProviders();
    }
}
