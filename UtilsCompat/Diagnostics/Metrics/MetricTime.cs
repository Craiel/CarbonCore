namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public class MetricTime : MetricLong
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricTime(long startTime, int id)
            : base(id)
        {
            this.StartTime = startTime;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long StartTime { get; private set; }
        
        public void Measure(long currentTime)
        {
            long elapsed = currentTime - this.StartTime;
            this.Add(elapsed);
        }

        public void Reset(long elapsed)
        {
            this.Reset();
            this.Add(elapsed);
        }
    }
}
