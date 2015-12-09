namespace CarbonCore.Utils.Diagnostics.Metrics
{
    using CarbonCore.Utils.Contracts.Diagnostics;

    public abstract class Metric<T> : IMetric
        where T : struct
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected Metric(int id)
        {
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Id { get; private set; }
        
        public long Count { get; protected set; }

        public T Total { get; protected set; }

        public T Min { get; protected set; }

        public T Max { get; protected set; }

        public T Average { get; protected set; }

        public abstract void Add(T value);

        public virtual void Reset()
        {
            this.Count = 0;

            this.Total = default(T);
            this.Min = default(T);
            this.Max = default(T);
            this.Average = default(T);
        }

        public void Add(IMetric other)
        {
            // Check if there is something to add
            if (other.Count <= 0)
            {
                return;
            }

            this.DoAdd((Metric<T>)other);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void DoAdd(Metric<T> other);
    }
}
