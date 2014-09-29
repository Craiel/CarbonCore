namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic;

    public interface IFileServiceProvider : IDisposable
    {
        bool IsInitialized { get; }

        long Capacity { get; }
        long Used { get; }

        long BytesRead { get; }
        long BytesWritten { get; }

        long EntriesDeleted { get; }

        void Load(FileEntryKey key, out byte[] data);
        void Save(FileEntryKey key, byte[] data);
        void Delete(FileEntryKey key);

        int Cleanup();

        void Initialize();

        IList<FileEntryKey> GetFiles(bool includeDeleted = false);

        void SetVersion(FileEntryKey key, int version);
        int GetVersion(FileEntryKey key);

        void SetCreateDate(FileEntryKey key, DateTime date);
        DateTime GetCreateDate(FileEntryKey key);

        void SetModifiedDate(FileEntryKey key, DateTime date);
        DateTime GetModifiedDate(FileEntryKey key);

        void GetMetadata(FileEntryKey key, int metadataKey, out int? value, out string stringValue);
        void SetMetadata(FileEntryKey key, int metadataKey, int? value = null, string stringValue = null);
    }
}
