namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    using CarbonCore.Utils.Compat.Contracts;
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
            SetInstance(new CarbonDiagnostics<TraceLog>());
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

        public static MetricSection BeginMeasure()
        {
            return instance.BeginMeasure();
        }

        public static void TakeMeasure(MetricSection section)
        {
            instance.TakeMeasure(section);
        }

        public static void ResetMeasure(MetricSection section)
        {
            instance.ResetMeasure(section);
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
            GetThreadContext().Error(PreformatMessage(exception.Message));
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

            instance.RegisterLogContext(threadId, string.Format("({0}) {1}", threadId, name));
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

            instance.UnregisterLogContext(threadId);
        }

        public static void SetMute(int managedThreadId, bool mute = true)
        {
            instance.SetMute(managedThreadId, mute);
        }

        public static void TraceMeasure(MetricSection section, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(message);
            builder.AppendFormat("  {0} measures\n", section.Metric.Count);
            builder.AppendFormat("  -> {0} Total, {1}ms\n", section.Metric.Total, GetTimeInMS(section.Metric.Total));
            builder.AppendFormat("  -> {0} Min, {1}ms\n", section.Metric.Min, GetTimeInMS(section.Metric.Min));
            builder.AppendFormat("  -> {0} Max, {1}ms\n", section.Metric.Max, GetTimeInMS(section.Metric.Max));
            builder.AppendFormat("  -> {0} Avg, {1}ms\n", section.AverageTime, GetTimeInMS(section.AverageTime));

            Info(builder.ToString());
        }

        public static float GetTimeInMS(long ticks)
        {
            return (ticks / (float)Stopwatch.Frequency) * 1000;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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
