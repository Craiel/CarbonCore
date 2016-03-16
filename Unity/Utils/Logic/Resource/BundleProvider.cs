namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Unity.Utils.Data;
    using CarbonCore.Unity.Utils.Logic.Enums;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.IO;

    using UnityEngine;

    public delegate void OnBundleLoadingDelegate(BundleLoadInfo key);
    public delegate void OnBundleLoadedDelegate(BundleLoadInfo key, long loadTime);

    public class BundleProvider : UnitySingleton<BundleProvider>
    {
        private readonly IDictionary<BundleLoadInfo, AssetBundle> bundles;
        private readonly IDictionary<BundleLoadInfo, CarbonFile> bundleFiles;

        private readonly IDictionary<BundleKey, BundleLoadInfo> loadInfoMap;

        private readonly Queue<BundleLoadInfo> currentPendingLoads;

        private readonly IDictionary<BundleKey, long> history;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleProvider()
        {
            this.bundles = new Dictionary<BundleLoadInfo, AssetBundle>();
            this.bundleFiles = new Dictionary<BundleLoadInfo, CarbonFile>();
            this.loadInfoMap = new Dictionary<BundleKey, BundleLoadInfo>();

            this.currentPendingLoads = new Queue<BundleLoadInfo>();

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

        public void RegisterBundle(BundleKey key, CarbonFile file, BundleLoadFlags flags = BundleLoadFlags.None)
        {
            if (this.loadInfoMap.ContainsKey(key))
            {
                // We already got this bundle, skip
                return;
            }

            var info = new BundleLoadInfo(key, flags);
            this.DoRegisterBundle(info, file);
            this.currentPendingLoads.Enqueue(info);
        }

        public void RegisterLoadedBundle(BundleKey key, AssetBundle bundle, BundleLoadFlags flags = BundleLoadFlags.None)
        {
            BundleLoadInfo info;
            if (!this.loadInfoMap.TryGetValue(key, out info))
            {
                // Register the new entry
                info = new BundleLoadInfo(key, flags);
                this.DoRegisterBundle(info, null);
            }

            // Update the bundle
            this.bundles[info] = bundle;
        }

        public void RegisterLazyBundle(BundleKey key, CarbonFile file, BundleLoadFlags flags = BundleLoadFlags.None)
        {
            if (this.loadInfoMap.ContainsKey(key))
            {
                // We already got this bundle, skip
                return;
            }

            var info = new BundleLoadInfo(key, flags);
            this.DoRegisterBundle(info, file);
        }

        public void UnregisterBundle(BundleKey key)
        {
            BundleLoadInfo info;
            if (!this.loadInfoMap.TryGetValue(key, out info))
            {
                // Bundle is not registered
                return;
            }
            
            this.bundles.Remove(info);
            this.bundleFiles.Remove(info);
            this.loadInfoMap.Remove(key);
        }

        public AssetBundle GetBundle(BundleKey key)
        {
            BundleLoadInfo info;
            if (!this.loadInfoMap.TryGetValue(key, out info))
            {
                // Bundle is not registered
                return null;
            }

            AssetBundle result;
            if (this.bundles.TryGetValue(info, out result))
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
            foreach (BundleLoadInfo info in this.bundles.Keys)
            {
                if (info.Key.Bundle.Equals(file.GetPath(), StringComparison.OrdinalIgnoreCase))
                {
                    return info.Key;
                }
            }

            return null;
        }

        public bool LoadBundleImmediate(BundleKey key)
        {
            BundleLoadInfo info;
            if (!this.loadInfoMap.TryGetValue(key, out info))
            {
                Diagnostic.Error("Bundle was not registered, can not load immediate: {0}", key);
                return false;
            }
            
            return this.DoLoadBundleImmediate(info);
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
                BundleLoadInfo info = this.currentPendingLoads.Dequeue();
                if (this.bundles[info] != null)
                {
                    // Skip this bundle, it was already loaded
                    return true;
                }

                CarbonFile file = this.bundleFiles[info];

                this.CurrentRequest = new BundleLoadRequest(info, file);
                
                if (this.BundleLoading != null)
                {
                    this.BundleLoading(info);
                }

                return true;
            }

            return false;
        }

        public void LoadImmediate()
        {
            while (this.currentPendingLoads.Count > 0)
            {
                BundleLoadInfo info = this.currentPendingLoads.Dequeue();
                this.DoLoadBundleImmediate(info);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool DoLoadBundleImmediate(BundleLoadInfo info)
        {
            if (this.bundles[info] != null)
            {
                // Already loaded, nothing to do
                return true;
            }

            var request = new BundleLoadRequest(info, this.bundleFiles[info]);
            request.LoadImmediate();
            this.FinalizeBundle(request);
            return true;
        }

        private void DoRegisterBundle(BundleLoadInfo info, CarbonFile file)
        {
            this.bundles.Add(info, null);
            this.bundleFiles.Add(info, file);

            this.loadInfoMap.Add(info.Key, info);
        }

        private void FinalizeBundle(BundleLoadRequest request)
        {
            AssetBundle bundle = request.GetBundle();
            MetricTime metric = request.Metric;

            this.bundles[request.Info] = bundle;
            Diagnostic.TakeTimeMeasure(metric);

            if (this.EnableHistory)
            {
                this.history.Add(request.Info.Key, metric.Total);
            }

            if (this.BundleLoaded != null)
            {
                this.BundleLoaded(request.Info, metric.Total);
            }
        }
    }
}
