namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    public interface IBufferedDataset : IBufferedDatasetReadOnly
    {
        void AddInstance(IBufferedDataEntry instance);

        void RemoveInstance(IBufferedDataEntry instance);
        
        void SetInstanceKey<T>(IBufferedDataEntry instance, T key);

        void Reset();
    }
}