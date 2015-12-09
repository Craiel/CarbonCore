namespace CarbonCore.Utils.Unity.Contracts.BufferedData
{
    using System.Collections.Generic;
    
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.Unity.Contracts;

    public interface IBufferedDatasetReadOnly : IRefCountedObject
    {
        int Id { get; }

        T GetInstance<T>(object key = null) where T : IDataEntry;
        IList<T> GetInstances<T>(object key = null) where T : IDataEntry;
    }
}
