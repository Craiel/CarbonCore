namespace CarbonCore.Utils.Compat.Diagnostics
{
    public class MetricSection
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricSection(long startTime)
        {
            this.StartTime = startTime;

            this.Min = long.MaxValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long StartTime { get; private set; }

        public long Elapsed { get; private set; }

        public long Count { get; set; }

        public long AverageTime { get; private set; }

        public long TotalTime { get; private set; }

        public long Min { get; private set; }

        public long Max { get; private set; }

        public void Measure(long currentTime)
        {
            long elapsed = currentTime - this.StartTime;
            this.Elapsed = elapsed;
            this.TotalTime += elapsed;
            this.Count++;
            this.AverageTime = this.TotalTime / this.Count;

            if (this.Min > elapsed)
            {
                this.Min = elapsed;
            }

            if (this.Max < elapsed)
            {
                this.Max = elapsed;
            }
        }

        public void Reset(long elapsed)
        {
            this.Elapsed = elapsed;
        }
    }
}
