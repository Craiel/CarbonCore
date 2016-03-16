namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

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
