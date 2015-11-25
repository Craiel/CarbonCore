namespace CarbonCore.Utils.Unity.Logic.BufferedData.Commands
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

    public class BufferCommandWriteSharedInstance : IBufferedDataCommand
    {
        private readonly object key;
        private readonly IDataEntry instance;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferCommandWriteSharedInstance(IDataEntry instance, object key = null)
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
