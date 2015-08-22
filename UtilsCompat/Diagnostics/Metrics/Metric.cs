namespace CarbonCore.Utils.Compat.Diagnostics.Metrics
{
    public abstract class Metric<T>
        where T : struct
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Count { get; protected set; }

        public T Total { get; protected set; }

        public T Min { get; protected set; }

        public T Max { get; protected set; }

        public T Average { get; protected set; }

        public abstract void Add(T value);

        public virtual void Reset()
        {
            this.Count = default(T);
            this.Total = default(T);
            this.Min = default(T);
            this.Max = default(T);
            this.Average = default(T);
        }
    }
}
