namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public class MetricFloat : Metric<float>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricFloat(int id)
            : base(id)
        {
            this.Min = float.MaxValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Add()
        {
            this.Add(1.0f);
        }

        public override void Add(float value)
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

            this.Min = float.MaxValue;
        }

        public static MetricFloat operator +(MetricFloat first, MetricFloat second)
        {
            var diff = new MetricFloat(first.Id)
            {
                Count = first.Count + second.Count,
                Total = first.Total + second.Total,
                Min = first.Min + second.Min,
                Max = first.Max + second.Max
            };

            diff.UpdateAverage();

            return diff;
        }

        public static MetricFloat operator -(MetricFloat first, MetricFloat second)
        {
            var diff = new MetricFloat(first.Id)
            {
                Count = first.Count - second.Count,
                Total = first.Total - second.Total,
                Min = first.Min - second.Min,
                Max = first.Max - second.Max
            };

            diff.UpdateAverage();

            return diff;
        }

        public void UpdateAverage()
        {
            if (this.Count > 0)
            {
                this.Average = this.Total / this.Count;
            }
            else
            {
                this.Total = this.Average;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoAdd(Metric<float> other)
        {
            this.Count += other.Count;
            this.Total += other.Total;
            if (this.Min > other.Min)
            {
                this.Min = other.Min;
            }

            if (this.Max < other.Max)
            {
                this.Max = other.Max;
            }

            this.UpdateAverage();
        }
    }
}
