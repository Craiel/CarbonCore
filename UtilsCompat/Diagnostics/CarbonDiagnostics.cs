﻿namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Contracts.Diagnostics;

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
        public void RegisterMetric<TN>(int id)
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

        public TN GetFullMetric<TN>(int id) where TN : IMetric
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

        public void UnregisterMetric(int id)
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

        public void RegisterMetricContext(int id)
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

        public void UnregisterMetricContext(int id)
        {
            lock (this.metricProviders)
            {
                this.metricProviders.Remove(id);
            }
        }

        public IMetricProvider GetMetricProvider(int id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return this.metricProviders[id];
        }
        
        public void RegisterLogContext(int id, string name)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Add(id, (T)Activator.CreateInstance(typeof(T), name));
            }
        }

        public void UnregisterLogContext(int id)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Remove(id);
            }
        }

        public ILog GetLogContext(int id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return this.contextLogs[id];
        }
    }
}
