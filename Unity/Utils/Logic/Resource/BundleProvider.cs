namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Unity.Data;

    using UnityEngine;

    public delegate void OnBundleLoadingDelegate(BundleKey key);
    public delegate void OnBundleLoadedDelegate(BundleKey key, long loadTime);

    public class BundleProvider : UnitySingleton<BundleProvider>
    {
        private readonly IDictionary<BundleKey, AssetBundle> bundles;
        private readonly IDictionary<BundleKey, CarbonFile> bundleFiles;

        private readonly Queue<BundleKey> currentPendingLoads;

        private readonly IDictionary<BundleKey, long> history;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleProvider()
        {
            this.bundles = new Dictionary<BundleKey, AssetBundle>();
            this.bundleFiles = new Dictionary<BundleKey, CarbonFile>();

            this.currentPendingLoads = new Queue<BundleKey>();

            this.history = new Dictionary<BundleKey, long>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event OnBundleLoadingDelegate BundleLoading;
        public event OnBundleLoadedDelegate BundleLoaded;

        public int PendingForLoad
        {
            get
            {
                return this.currentPendingLoads.Count;
            }
        }

        public int BundlesLoaded { get; private set; }

        public bool EnableHistory { get; set; }

        public BundleLoadRequest CurrentRequest { get; private set; }

        public void RegisterBundle(BundleKey key, CarbonFile file)
        {
            if (this.bundles.ContainsKey(key))
            {
                // We already got this bundle, skip
                return;
            }

            this.DoRegisterBundle(key, file);
            this.currentPendingLoads.Enqueue(key);
        }

        public void RegisterLoadedBundle(BundleKey key, AssetBundle bundle)
        {
            if (this.bundles.ContainsKey(key))
            {
                this.bundles[key] = bundle;
            }
            else
            {
                this.bundles.Add(key, bundle);
            }
        }

        public void RegisterLazyBundle(BundleKey key, CarbonFile file)
        {
            if (this.bundles.ContainsKey(key))
            {
                // We already got this bundle, skip
                return;
            }

            this.DoRegisterBundle(key, file);
        }

        public void UnregisterBundle(BundleKey key)
        {
            if (!this.bundles.ContainsKey(key))
            {
                // Bundle is not registered
                return;
            }

            this.bundles.Remove(key);
            this.bundleFiles.Remove(key);
        }

        public AssetBundle GetBundle(BundleKey key)
        {
            AssetBundle result;
            if (this.bundles.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        public IDictionary<BundleKey, long> GetHistory()
        {
            return this.history;
        }

        public BundleKey? GetBundleKey(CarbonFile file)
        {
            foreach (BundleKey key in this.bundles.Keys)
            {
                if (key.Bundle.Equals(file.GetPath(), StringComparison.OrdinalIgnoreCase))
                {
                    return key;
                }
            }

            return null;
        }

        public bool LoadBundleImmediate(BundleKey key)
        {
            if (!this.bundles.ContainsKey(key))
            {
                Diagnostic.Error("Bundle was not registered, can not load immediate: {0}", key);
                return false;
            }

            if (this.bundles[key] != null)
            {
                // Already loaded, nothing to do
                return true;
            }

            var request = new BundleLoadRequest(key, this.bundleFiles[key]);
            request.LoadImmediate();
            this.FinalizeBundle(request);
            return true;
        }

        public bool ContinueLoad()
        {
            if (this.CurrentRequest != null)
            {
                if (this.CurrentRequest.ContinueLoading())
                {
                    return true;
                }

                // We are done with this bundle
                this.FinalizeBundle(this.CurrentRequest);

                this.CurrentRequest = null;
            }

            if (this.currentPendingLoads.Count > 0)
            {
                BundleKey key = this.currentPendingLoads.Dequeue();
                if (this.bundles[key] != null)
                {
                    // Skip this bundle, it was already loaded
                    return true;
                }

                CarbonFile file = this.bundleFiles[key];

                this.CurrentRequest = new BundleLoadRequest(key, file);
                
                if (this.BundleLoading != null)
                {
                    this.BundleLoading(key);
                }

                return true;
            }

            return false;
        }

        public void LoadImmediate()
        {
            while (this.currentPendingLoads.Count > 0)
            {
                BundleKey key = this.currentPendingLoads.Dequeue();
                this.LoadBundleImmediate(key);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoRegisterBundle(BundleKey key, CarbonFile file)
        {
            this.bundles.Add(key, null);
            this.bundleFiles.Add(key, file);
        }

        private void FinalizeBundle(BundleLoadRequest request)
        {
            BundleKey key = request.Key;
            AssetBundle bundle = request.GetBundle();
            MetricTime metric = request.Metric;

            this.bundles[key] = bundle;
            Diagnostic.TakeTimeMeasure(metric);

            if (this.EnableHistory)
            {
                this.history.Add(key, metric.Total);
            }

            if (this.BundleLoaded != null)
            {
                this.BundleLoaded(key, metric.Total);
            }
        }
    }
}
