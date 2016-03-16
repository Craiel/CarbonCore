namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandWriteSharedInstance : IBufferedDataCommand
    {
        private readonly object key;
        private readonly IBufferedDataEntry instance;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferCommandWriteSharedInstance(IBufferedDataEntry instance, object key = null)
        {
            this.key = key;
            this.instance = instance;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Execute(IBufferedDataset target)
        {
            target.AddInstance(this.instance);

            if (this.key != null)
            {
                target.SetInstanceKey(this.instance, this.key);
            }
        }
    }
}
