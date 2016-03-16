namespace CarbonCore.Unity.Utils.Logic.BufferedData
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferedDataPendingCommand
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferedDataPendingCommand(long id, IBufferedDataCommand command)
        {
            this.Id = id;
            this.Command = command;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Id { get; private set; }

        public IBufferedDataCommand Command { get; private set; }
    }
}
