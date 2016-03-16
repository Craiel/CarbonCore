namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    using CarbonCore.Unity.Utils.Logic.BufferedData;

    public interface IBufferedDataPool
    {
        int ActiveDataset { get; }

        int DatasetCount { get; }

        int DedicatedDatasetCount { get; }
        
        long CurrentCommand { get; }

        long LatestCommand { get; }

        int CommandErrors { get; }

        void Initialize(BufferedDataPoolSettings settings);

        void Update();

        DataSnapshot GetDedicatedData(int id = 0);
        DataSnapshot GetData();

        long Enqueue(IBufferedDataCommand command);

        void Commit();
        void Reset();
    }
}
