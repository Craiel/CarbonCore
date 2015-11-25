namespace CarbonCore.Utils.Unity.Logic.BufferedData.Commands
{
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

    public class BufferCommandReset : IBufferedDataCommand
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Execute(IBufferedDataset target)
        {
            target.Reset();
        }
    }
}
