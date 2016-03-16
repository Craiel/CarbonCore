namespace CarbonCore.Utils.Unity.Contracts.BufferedData
{
    using System.Collections.Generic;
    
    using CarbonCore.Utils.Unity.Contracts;

    public interface IBufferedDatasetReadOnly : IRefCountedObject
    {
        int Id { get; }

        T GetInstance<T>(object key = null) where T : IBufferedDataEntry;
        IList<T> GetInstances<T>(object key = null) where T : IBufferedDataEntry;
    }
}
