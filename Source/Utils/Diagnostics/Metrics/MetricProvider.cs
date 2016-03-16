namespace CarbonCore.Utils.Diagnostics.Metrics
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Utils.Contracts.Diagnostics;

    public class MetricProvider : IMetricProvider
    {
        private readonly Stopwatch stopwatch;

        private readonly IDictionary<int, IMetric> metrics;

        private int nextTimeMeasureId = 9000;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MetricProvider()
        {
            this.metrics = new Dictionary<int, IMetric>();

            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Register(IMetric metric)
        {
            this.metrics.Add(metric.Id, metric);
        }

        public void Unregister(int metric)
        {
            this.metrics.Remove(metric);
        }

        public IMetric GetMetric(int id)
        {
            return this.metrics[id];
        }

        public IList<IMetric> GetActiveMetrics()
        {
            return new List<IMetric>(this.metrics.Values);
        }

        public MetricTime BeginTimeMeasure()
        {
            return new MetricTime(this.stopwatch.ElapsedTicks, ++this.nextTimeMeasureId);
        }

        public void TakeTimeMeasure(MetricTime section)
        {
            section.Measure(this.stopwatch.ElapsedTicks);
        }

        public void ResetTimeMeasure(MetricTime section)
        {
            section.Reset(this.stopwatch.ElapsedTicks);
        }
    }
}
