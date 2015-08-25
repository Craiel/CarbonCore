namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public class MetricInt : Metric<int>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricInt(int id)
            : base(id)
        {
            this.Min = int.MaxValue;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Add()
        {
            this.Add(1);
        }

        public override void Add(int value)
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

            this.Average = (int)(this.Total / this.Count);
        }

        public override void Reset()
        {
            base.Reset();

            this.Min = int.MaxValue;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoAdd(Metric<int> other)
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

            this.Average = (int)(this.Total / this.Count);
        }
    }
}
