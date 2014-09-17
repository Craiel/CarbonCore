namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic;

    public interface IFileService : IDisposable
    {
        int FileCount { get; }

        IFileEntryData Load(FileEntry key);
        bool Save(FileEntry key, IFileEntryData data, string internalFileName = null, IFileServiceProvider targetProvider = null);
        bool Delete(FileEntry key);

        void AddProvider(IFileServiceProvider provider);
        void RemoveProvider(IFileServiceProvider provider);

        bool CheckForUpdate(FileEntry key);

        IList<FileEntry> GetFileEntries();

        IList<IFileServiceProvider> GetProviders();
    }
}
