namespace CarbonCore.Utils.Unity.Contracts.BufferedData
{
    public interface IBufferedDataCommand
    {
        void Execute(IBufferedDataset target);
    }
}