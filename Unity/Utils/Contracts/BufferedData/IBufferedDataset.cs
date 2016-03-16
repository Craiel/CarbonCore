namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    using CarbonCore.ContentServices.Contracts;

    public interface IBufferedDataset : IBufferedDatasetReadOnly
    {
        void AddInstance(IDataEntry instance);

        void RemoveInstance(IDataEntry instance);
        
        void SetInstanceKey<T>(IDataEntry instance, T key);

        void Reset();
    }
}