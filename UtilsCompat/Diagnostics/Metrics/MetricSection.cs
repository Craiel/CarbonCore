namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public class MetricSection
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricSection(long startTime)
        {
            this.StartTime = startTime;
            this.Metric = new MetricLong();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long StartTime { get; private set; }

        public MetricLong Metric { get; private set; }
        
        public long AverageTime { get; private set; }
        
        public void Measure(long currentTime)
        {
            long elapsed = currentTime - this.StartTime;
            this.Metric.Add(elapsed);
        }

        public void Reset(long elapsed)
        {
            this.Metric.Reset();
            this.Metric.Add(elapsed);
        }
    }
}
