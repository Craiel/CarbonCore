namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;

    public interface IFileService : IDisposable
    {
        int FileCount { get; }

        T Load<T>(IFileInfo key) where T : IFileEntry;
        bool Save(IFileInfo key, IFileEntry data);
        bool Delete(IFileInfo key);
        bool Reload(IFileInfo key, IFileEntry target);
        void Release(IFileInfo key, ref IFileEntry data);

        void AddProvider(IFileServiceProvider provider);
        void RemoveProvider(IFileServiceProvider provider);

        bool CheckForUpdate(IFileInfo key);

        IList<IFileInfo> GetFileInfos();

        IList<IFileServiceProvider> GetProviders();
    }
}
