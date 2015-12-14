namespace Assets.Scripts.Game
{
    using Assets.Scripts.Enums;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;

    public static class GameMetrics
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static GameMetrics()
        {
            // Diagnostic.RegisterMetric<MetricLong>((int)BackendMetric.EngineBuffereCommandExecuteCount);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void AddLong(BackendMetric metric, long value = 1)
        {
            Diagnostic.GetMetric<MetricLong>((int)metric).Add(value);
        }

        public static void AddFloat(BackendMetric metric, float value = 1.0f)
        {
            Diagnostic.GetMetric<MetricFloat>((int)metric).Add(value);
        }

        public static MetricLong GetLongMetric(BackendMetric metric)
        {
            return Diagnostic.GetFullMetric<MetricLong>((int)metric);
        }

        public static MetricFloat GetFloatMetric(BackendMetric metric)
        {
            return Diagnostic.GetFullMetric<MetricFloat>((int)metric);
        }
    }
}
