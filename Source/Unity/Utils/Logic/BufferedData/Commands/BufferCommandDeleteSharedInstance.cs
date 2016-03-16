namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandDeleteSharedInstance : IBufferedDataCommand
    {
        private readonly IBufferedDataEntry instance;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferCommandDeleteSharedInstance(IBufferedDataEntry instance)
        {
            this.instance = instance;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Execute(IBufferedDataset target)
        {
            target.RemoveInstance(this.instance);
        }
    }
}
