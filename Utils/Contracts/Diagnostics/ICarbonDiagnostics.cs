﻿namespace CarbonCore.Utils.Contracts.Diagnostics
{
    public interface ICarbonDiagnostics
    {
        // Metrics
        void RegisterMetric<TN>(int id) where TN : IMetric;

        TN GetFullMetric<TN>(int id) where TN : IMetric;

        void UnregisterMetric(int id);

        void RegisterMetricContext(int id);

        void UnregisterMetricContext(int id);

        IMetricProvider GetMetricProvider(int id);

        // Logging
        bool HasLogContext(int id);

        void RegisterLogContext(int id, string name);

        void UnregisterLogContext(int id);

        ILog GetLogContext(int id);
    }
}
