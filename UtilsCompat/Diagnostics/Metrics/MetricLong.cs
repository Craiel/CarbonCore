namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public class MetricLong : Metric<long>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricLong()
        {
            this.Min = long.MaxValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Add(long value)
        {
            this.Count++;
            this.Total += value;

            if (this.Min > value)
            {
                this.Min = value;
            }

            if (this.Max < value)
            {
                this.Max = value;
            }

            this.Average = this.Total / this.Count;
        }

        public override void Reset()
        {
            base.Reset();

            this.Min = long.MaxValue;
        }
    }
}
