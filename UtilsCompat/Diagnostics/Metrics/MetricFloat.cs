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

            this.Average = this.Total / this.Count;
        }
    }
}
