namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IFileServiceProvider : IDisposable
    {
        bool IsInitialized { get; }

        long Capacity { get; }
        long Used { get; }

        long BytesRead { get; }
        long BytesWritten { get; }

        bool Load(string hash, out byte[] data);
        bool Save(string hash, byte[] data);
        bool Delete(string hash);

        int Cleanup();

        void Initialize();

        IList<IFileEntry> GetFiles();
    }
}
