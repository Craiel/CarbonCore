namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IFileService : IDisposable
    {
        int FileCount { get; }

        T Load<T>(ref IFileInfo info) where T : IFileEntry;
        bool Save(ref IFileInfo info, IFileEntry entry);
        bool Delete(ref IFileInfo info);

        void AddProvider(IFileServiceProvider provider);
        void RemoveProvider(IFileServiceProvider provider);

        bool CheckForUpdate(IFileEntry entry);

        IList<IFileInfo> GetFileInfos();

        IList<IFileServiceProvider> GetProviders();
    }
}
