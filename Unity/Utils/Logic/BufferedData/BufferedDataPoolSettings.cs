namespace CarbonCore.Utils.Unity.Logic.BufferedData
{
    public class BufferedDataPoolSettings
    {
        public BufferedDataPoolSettings()
        {
            this.MinDatasetCount = 1;
            this.MaxDatasetCount = 1;
            this.DedicatedDatasets = 0;
        }

        public int MinDatasetCount { get; set; }
        public int MaxDatasetCount { get; set; }

        public int DedicatedDatasets { get; set; }

        public bool UseDynamicDatasetAllocation { get; set; }
    }
}
