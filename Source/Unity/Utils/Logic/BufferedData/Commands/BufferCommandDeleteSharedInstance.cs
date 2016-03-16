namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandDeleteSharedInstance : IBufferedDataCommand
    {
        private readonly IDataEntry instance;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferCommandDeleteSharedInstance(IDataEntry instance)
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
