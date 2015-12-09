namespace CarbonCore.Utils.Unity.Logic.BufferedData.Commands
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

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
