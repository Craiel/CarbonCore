namespace CarbonCore.ContentServices.Sql.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Data;
    using CarbonCore.ContentServices.Sql.Logic;

    public interface IFileService : IDisposable
    {
        int FileCount { get; }

        IFileServiceProvider DefaultProvider { get; set; }

        FileEntryData Load(FileEntryKey key);
        void Save(FileEntryKey key, FileEntryData data, IFileServiceProvider targetProvider = null);
        void Update(FileEntryKey key, FileEntryData data, bool autoIncrementVersion = true);
        void Delete(FileEntryKey key);
        void Move(FileEntryKey key, IFileServiceProvider targetProvider);

        void AddProvider(IFileServiceProvider provider);
        void RemoveProvider(IFileServiceProvider provider);
        
        IList<FileEntryKey> GetFileEntries();

        IFileServiceProvider GetProvider(FileEntryKey key);
        IList<IFileServiceProvider> GetProviders();

        void SetVersion(FileEntryKey key, int version);
        int GetVersion(FileEntryKey key);

        void SetCreateDate(FileEntryKey key, DateTime date);
        DateTime GetCreateDate(FileEntryKey key);

        void SetModifiedDate(FileEntryKey key, DateTime date);
        DateTime GetModifiedDate(FileEntryKey key);
        
        int Cleanup();
    }
}
