namespace CarbonCore.Utils.Unity.Contracts.BufferedData
{
    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Logic.BufferedData;

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
