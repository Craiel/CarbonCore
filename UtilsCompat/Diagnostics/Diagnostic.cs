namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;

    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Contracts.Diagnostics;
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;
    using CarbonCore.Utils.Compat.Threading;

    public class Diagnostic
    {
        private static readonly IDictionary<int, EngineTime> ThreadTimes = new Dictionary<int, EngineTime>();

        private static ICarbonDiagnostics instance;

        private static volatile bool enableTimeStamp;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        static Diagnostic()
        {
            // Initialize the default diagnostic instance
            SetInstance(new CarbonDiagnostics<TraceLog, MetricProvider>());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void SetInstance<T>(T diagnosticInstance)
            where T : ICarbonDiagnostics
        {
            instance = diagnosticInstance;
        }

        public static bool EnableTimeStamp
        {
            get
            {
                return enableTimeStamp;
            }

            set
            {
                enableTimeStamp = value;
            }
        }

        public static void RegisterMetric<T>(int id)
            where T : IMetric
        {
            instance.RegisterMetric<T>(id);
        }

        public static T GetMetric<T>(int id)
            where T : IMetric
        {
            return (T)GetMetricContext().GetMetric(id);
        }

        public static T GetFullMetric<T>(int id)
            where T : IMetric
        {
            return instance.GetFullMetric<T>(id);
        }

        public static MetricTime BeginTimeMeasure()
        {
            return GetMetricContext().BeginTimeMeasure();
        }

        public static void TakeTimeMeasure(MetricTime metric)
        {
            GetMetricContext().TakeTimeMeasure(metric);
        }

        public static void ResetTimeMeasure(MetricTime metric)
        {
            GetMetricContext().ResetTimeMeasure(metric);
        }

        public static void Debug(string message, params object[] args)
        {
            GetThreadContext().Debug(PreformatMessage(message), args);
        }

        public static void Warning(string message, params object[] args)
        {
            GetThreadContext().Warning(PreformatMessage(message), args);
        }

        public static void Exception(Exception exception)
        {
            GetThreadContext().LogException(exception);
        }

        public static void Error(string message)
        {
            GetThreadContext().Error(PreformatMessage(message));
        }

        public static void Error(string message, params object[] args)
        {
            GetThreadContext().Error(PreformatMessage(message), args);
        }
        
        public static void Info(string message, params object[] args)
        {
            GetThreadContext().Info(PreformatMessage(message), args);
        }

        public static void Assert(bool condition, string message = null)
        {
            GetThreadContext().Assert(condition, PreformatMessage(message ?? string.Empty));
        }

        public static void RegisterThread(string name, EngineTime time = null)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (time != null)
            {
                lock (ThreadTimes)
                {
                    ThreadTimes.Add(threadId, time);
                }
            }

            string threadContextName = string.Format("({0}) {1}", threadId, name);
            instance.RegisterMetricContext(threadId);
            instance.RegisterLogContext(threadId, threadContextName);

            Info("Registered Thread {0}", threadContextName);
        }

        public static void UnregisterThread()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (ThreadTimes)
            {
                if (ThreadTimes.ContainsKey(threadId))
                {
                    ThreadTimes.Remove(threadId);
                }
            }

            instance.UnregisterMetricContext(threadId);
            instance.UnregisterLogContext(threadId);
        }

        public static bool GetMute(int managedThreadId)
        {
            return instance.GetLogContext(managedThreadId).IsMuted;
        }

        public static void SetMute(int managedThreadId, bool mute = true)
        {
            instance.GetLogContext(managedThreadId).IsMuted = mute;
        }

        public static void TraceMeasure(MetricTime metric, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendFormat("  {0} measures\n", metric.Count);
            builder.AppendFormat("  -> {0} Total, {1}ms\n", metric.Total, GetTimeInMS(metric.Total));
            builder.AppendFormat("  -> {0} Min, {1}ms\n", metric.Min, GetTimeInMS(metric.Min));
            builder.AppendFormat("  -> {0} Max, {1}ms\n", metric.Max, GetTimeInMS(metric.Max));
            builder.AppendFormat("  -> {0} Avg, {1}ms\n", metric.Average, GetTimeInMS(metric.Average));

            Info(builder.ToString());
        }

        public static float GetTimeInMS(long ticks)
        {
            return (ticks / (float)Stopwatch.Frequency) * 1000;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static IMetricProvider GetMetricContext()
        {
            return instance.GetMetricProvider(Thread.CurrentThread.ManagedThreadId);
        }

        private static ILog GetThreadContext()
        {
            return instance.GetLogContext(Thread.CurrentThread.ManagedThreadId);
        }

        private static string PreformatMessage(string message)
        {
            if (enableTimeStamp)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                if (ThreadTimes.ContainsKey(threadId))
                {
                    return string.Format("{0} {1}", ThreadTimes[threadId].Time, message);
                }
            }

            return message;
        }
    }
}