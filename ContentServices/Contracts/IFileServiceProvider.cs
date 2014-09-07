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

        bool Load(IFileEntry key, out byte[] data);
        bool Save(IFileEntry key, byte[] data);

        void Initialize();

        IList<IFileEntry> GetFiles();
    }
}
