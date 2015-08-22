namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;

    public class CarbonDiagnostics<T> : ICarbonDiagnostics
        where T : ILog
    {
        private readonly Stopwatch stopwatch;

        private readonly IDictionary<int, T> contextLogs;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDiagnostics()
        {
            this.contextLogs = new Dictionary<int, T>();

            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public MetricSection BeginMeasure()
        {
            return new MetricSection(this.stopwatch.ElapsedTicks);
        }

        public void TakeMeasure(MetricSection section)
        {
            section.Measure(this.stopwatch.ElapsedTicks);
        }

        public void ResetMeasure(MetricSection section)
        {
            section.Reset(this.stopwatch.ElapsedTicks);
        }

        public virtual void RegisterLogContext(int id, string name)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Add(id, (T)Activator.CreateInstance(typeof(T), name));
            }
        }

        public virtual void UnregisterLogContext(int id)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Remove(id);
            }
        }

        public virtual ILog GetLogContext(int id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return this.contextLogs[id];
        }

        public virtual void SetMute(int managedThreadId, bool mute)
        {
            lock (this.contextLogs)
            {
                this.contextLogs[managedThreadId].IsMuted = mute;
            }
        }
    }
}
