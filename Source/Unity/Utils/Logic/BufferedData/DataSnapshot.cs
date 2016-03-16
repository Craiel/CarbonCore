namespace CarbonCore.Unity.Utils.Logic.BufferedData
{
    using CarbonCore.Unity.Utils.Contracts.BufferedData;
    using CarbonCore.Unity.Utils.Data;

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
