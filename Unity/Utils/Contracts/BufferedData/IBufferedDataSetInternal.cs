namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    internal interface IBufferedDataSetInternal : IEngineComponent, IBufferedDataset
    {
        void Execute(IBufferedDataCommand command);

        IBufferedDataSetInternal Clone();
    }
}
