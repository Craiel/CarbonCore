namespace CarbonCore.Utils.Compat.Contracts.Diagnostics
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Diagnostics.Metrics;

    public interface IMetricProvider
    {
        void Register(IMetric metric);

        void Unregister(int id);

        IMetric GetMetric(int id);

        IList<IMetric> GetActiveMetrics();

        MetricTime BeginTimeMeasure();

        void TakeTimeMeasure(MetricTime metric);

        void ResetTimeMeasure(MetricTime metric);
    }
}
