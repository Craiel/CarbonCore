namespace CarbonCore.Unity.Utils.Contracts.BufferedData
{
    public interface IBufferedDataCommand
    {
        void Execute(IBufferedDataset target);
    }
}