namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    using CarbonCore.Unity.Utils.Logic.BufferedData;

    public interface IBufferedData : IEngineComponent
    {
        long CurrentCommand { get; }
        long LatestCommand { get; }

        int CommandErrors { get; }

        DataSnapshot GetData();

        long Enqueue(IBufferedDataCommand command);

        void Commit();
        void Reset();
    }
}
