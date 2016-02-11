namespace CarbonCore.CFS.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using CarbonCore.Utils.IO;

    public delegate void CFSChangedDelegate(CarbonFile file);

    public interface ICFSInstance : IDisposable
    {
        bool IsInitialized { get; }

        ReadOnlyCollection<CarbonFile> Files { get; }

        Stream BeginRead(CarbonFile file);
        bool Read(CarbonFile file, out byte[] data);
        bool Read(CarbonFile file, out string data);

        void Store(CarbonFile file);
        void Store(CarbonFile file, Stream data);
        void Store(CarbonFile file, byte[] data);
        void Store(CarbonFile file, string data);

        void Delete(CarbonFile file);
        
        IList<CarbonFile> Find(ICFSFilter filter);

        void SetMetaData(CarbonFile file, int key, object value);
        void SetMetaData(CarbonFile file, IDictionary<int, object> data);

        object GetMetaData(CarbonFile file, int key);
        IDictionary<int, object> GetMetaData(CarbonFile file);
    }
}
