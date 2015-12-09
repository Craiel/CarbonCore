namespace CarbonCore.Tests.Compat.Utils
{
    using System;
    using System.Threading;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class DiagnosticTests
    {
        private const int FirstMetricId = 500;
        private const int SecondMetricId = 1000;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void TestThreadedTracing()
        {
            var first = new EngineThread(this.FirstThreadTracingMain, "First Thread", new EngineThreadSettings(2));
            first.Start();

            var second = new EngineThread(this.SecondThreadTracingMain, "Second Thread", new EngineThreadSettings(10));
            second.Start();

            Diagnostic.Error("Test Error message");

            Thread.Sleep(TimeSpan.FromSeconds(2));

            first.Shutdown();
            second.Shutdown();
        }

        [Test]
        public void TestThreadedMetrics()
        {
            Diagnostic.RegisterMetric<MetricLong>(FirstMetricId);
            Diagnostic.RegisterMetric<MetricFloat>(SecondMetricId);

            var first = new EngineThread(this.FirstThreadMetricsMain, "First Thread", new EngineThreadSettings(2));
            first.Start();

            var second = new EngineThread(this.SecondThreadMetricsMain, "Second Thread", new EngineThreadSettings(10));
            second.Start();

            MetricTime time = Diagnostic.BeginTimeMeasure();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            Diagnostic.TakeTimeMeasure(time);
            Diagnostic.TraceMeasure(time, "Overall Metric Test Time");

            MetricLong longResult = Diagnostic.GetFullMetric<MetricLong>(FirstMetricId);
            Assert.AreEqual(4, longResult.Count);

            MetricFloat floatResult = Diagnostic.GetFullMetric<MetricFloat>(SecondMetricId);
            Assert.GreaterOrEqual(floatResult.Count, 22);

            first.Shutdown();
            second.Shutdown();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool FirstThreadTracingMain(EngineTime time)
        {
            Diagnostic.Info("Info");
            return true;
        }

        private bool SecondThreadTracingMain(EngineTime time)
        {
            Diagnostic.Warning("Warning!!");
            return true;
        }

        private bool FirstThreadMetricsMain(EngineTime time)
        {
            Diagnostic.GetMetric<MetricLong>(FirstMetricId).Add(10);
            Diagnostic.GetMetric<MetricFloat>(SecondMetricId).Add(1f);
            return true;
        }

        private bool SecondThreadMetricsMain(EngineTime time)
        {
            Diagnostic.GetMetric<MetricFloat>(SecondMetricId).Add(5.5f);
            return true;
        }
    }
}
