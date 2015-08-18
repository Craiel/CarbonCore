namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System.Diagnostics;
    using System.Text;

    public static class Metrics
    {
        private static readonly Stopwatch Stopwatch;
        
        static Metrics()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public static MetricSection BeginMetric()
        {
            return new MetricSection(Stopwatch.ElapsedTicks);
        }

        public static void TakeMeasure(MetricSection metric)
        {
            metric.Measure(Stopwatch.ElapsedTicks);
        }

        public static void TraceMeasure(MetricSection metric, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendFormat("  {0} measures\n", metric.Count);
            builder.AppendFormat("  -> {0} Total, {1}ms\n", metric.TotalTime, GetTimeInMS(metric.TotalTime));
            builder.AppendFormat("  -> {0} Min, {1}ms\n", metric.Min, GetTimeInMS(metric.Min));
            builder.AppendFormat("  -> {0} Max, {1}ms\n", metric.Max, GetTimeInMS(metric.Max));
            builder.AppendFormat("  -> {0} Avg, {1}ms\n", metric.AverageTime, GetTimeInMS(metric.AverageTime));

            Trace.TraceInformation(builder.ToString());
        }

        private static float GetTimeInMS(long ticks)
        {
            return (ticks / (float)Stopwatch.Frequency) * 1000;
        }
    }
}
