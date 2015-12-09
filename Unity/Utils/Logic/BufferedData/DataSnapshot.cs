namespace CarbonCore.Utils.Unity.Logic.BufferedData
{
    using CarbonCore.Utils.Unity.Contracts.BufferedData;
    using CarbonCore.Utils.Unity.Data;

    public class DataSnapshot : RefCountedWeakReference<IBufferedDatasetReadOnly>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataSnapshot(IBufferedDatasetReadOnly target)
            : base(target)
        {
            this.BufferId = target.Id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int BufferId { get; private set; }
    }
}
