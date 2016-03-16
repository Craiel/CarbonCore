namespace CarbonCore.Unity.Tests.Data
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferTestEntryUpdateCommand : IBufferedDataCommand
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferTestEntryUpdateCommand(int targetValue)
        {
            this.TargetValue = targetValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int TargetValue { get; private set; }

        public void Execute(IBufferedDataset target)
        {
            var entry = target.GetInstance<BufferTestEntry>();
            entry.TestInt = this.TargetValue;
        }
    }
}
