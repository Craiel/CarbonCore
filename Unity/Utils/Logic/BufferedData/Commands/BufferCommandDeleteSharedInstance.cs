namespace CarbonCore.Utils.Unity.Logic.BufferedData.Commands
{
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

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
