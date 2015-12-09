namespace CarbonCore.Utils.Unity.Contracts.BufferedData
{
    using CarbonCore.Utils.Unity.Contracts;

    internal interface IBufferedDataSetInternal : IEngineComponent, IBufferedDataset
    {
        void Execute(IBufferedDataCommand command);

        IBufferedDataSetInternal Clone();
    }
}
