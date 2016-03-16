namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    using System.Collections.Generic;

    public interface IBufferedDatasetReadOnly : IRefCountedObject
    {
        int Id { get; }

        T GetInstance<T>(object key = null) where T : IBufferedDataEntry;
        IList<T> GetInstances<T>(object key = null) where T : IBufferedDataEntry;
    }
}
