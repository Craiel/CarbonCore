namespace CarbonCore.Tests.Unity.Data
{
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

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
