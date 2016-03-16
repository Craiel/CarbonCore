namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    using System.Collections.Generic;
    
    using CarbonCore.ContentServices.Contracts;

    public interface IBufferedDatasetReadOnly : IRefCountedObject
    {
        int Id { get; }

        T GetInstance<T>(object key = null) where T : IDataEntry;
        IList<T> GetInstances<T>(object key = null) where T : IDataEntry;
    }
}
