namespace CarbonCore.Utils.Diagnostics
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.Diagnostics;

    public class CarbonDiagnostics<T, TM> : ICarbonDiagnostics
        where T : ILog
        where TM : IMetricProvider
    {
        private readonly IDictionary<int, Type> metrics;
        private readonly IDictionary<int, T> contextLogs;
        private readonly IDictionary<int, TM> metricProviders;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDiagnostics()
        {
            this.metrics = new Dictionary<int, Type>();
            this.contextLogs = new Dictionary<int, T>();
            this.metricProviders = new Dictionary<int, TM>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void RegisterMetric<TN>(int id)
            where TN : IMetric
        {
            lock (this.metricProviders)
            {
                this.metrics.Add(id, typeof(TN));

                // Register the metric in all active providers
                foreach (int providerId in this.metricProviders.Keys)
                {
                    TN instance = (TN)Activator.CreateInstance(typeof(TN), id);
                    this.metricProviders[providerId].Register(instance);
                }
            }
        }

        public virtual TN GetFullMetric<TN>(int id) where TN : IMetric
        {
            lock (this.metricProviders)
            {
                TN result = (TN)Activator.CreateInstance(typeof(TN), id);
                foreach (int providerId in this.metricProviders.Keys)
                {
                    result.Add(this.metricProviders[providerId].GetMetric(id));
                }

                return result;
            }
        }

        public virtual void UnregisterMetric(int id)
        {
            lock (this.metricProviders)
            {
                this.metrics.Remove(id);

                foreach (int threadId in this.metricProviders.Keys)
                {
                    this.metricProviders[threadId].Unregister(id);
                }
            }
        }

        public virtual void RegisterMetricContext(int id)
        {
            lock (this.metricProviders)
            {
                IMetricProvider provider = Activator.CreateInstance<TM>();
                foreach (int metricId in this.metrics.Keys)
                {
                    IMetric instance = (IMetric)Activator.CreateInstance(this.metrics[metricId], metricId);
                    provider.Register(instance);
                }

                this.metricProviders.Add(id, (TM)provider);
            }
        }

        public virtual void UnregisterMetricContext(int id)
        {
            lock (this.metricProviders)
            {
                this.metricProviders.Remove(id);
            }
        }

        public virtual IMetricProvider GetMetricProvider(int id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return this.metricProviders[id];
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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected IDictionary<int, Type> Metrics
        {
            get
            {
                return this.metrics;
            }
        }

        protected IDictionary<int, T> ContextLogs
        {
            get
            {
                return this.contextLogs;
            }
        }

        protected IDictionary<int, TM> MetricProviders
        {
            get
            {
                return this.metricProviders;
            }
        }
    }
}
