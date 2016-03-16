namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using System.Threading;

    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandResetEvent : IBufferedDataCommand
    {
        private readonly ManualResetEvent resetEvent;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferCommandResetEvent(ManualResetEvent resetEvent)
        {
            this.resetEvent = resetEvent;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Execute(IBufferedDataset target)
        {
            this.resetEvent.Set();
        }
    }
}
