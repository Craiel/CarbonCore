namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.Unity.Utils.Data;
    using CarbonCore.Unity.Utils.Logic.Enums;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;

    using UnityEngine;

    public delegate void OnResourceLoadingDelegate(ResourceLoadInfo info);
    public delegate void OnResourceLoadedDelegate(ResourceLoadInfo info, long loadTime);

    public class ResourceProvider : UnitySingleton<ResourceProvider>
    {
        private const int DefaultRequestPoolSize = 30;

        private const int MaxConsecutiveSyncCallsInAsync = 20;

        private readonly ResourceMap<UnityEngine.Object> resourceMap;

        private readonly IDictionary<ResourceKey, int> referenceCount;
        
        private readonly Queue<ResourceLoadInfo> currentPendingLoads;
        private readonly IList<UnityEngine.Object> pendingInstantiations;

        private readonly ResourceRequestPool<ResourceLoadRequest> requestPool;
        
        private readonly IDictionary<ResourceKey, long> history;

        private readonly IDictionary<Type, ResourceKey> fallbackResources;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceProvider()
        {
            this.resourceMap = new ResourceMap<UnityEngine.Object>();
            this.referenceCount = new Dictionary<ResourceKey, int>();

            this.currentPendingLoads = new Queue<ResourceLoadInfo>();
            this.pendingInstantiations = new List<UnityEngine.Object>();

            this.requestPool = new ResourceRequestPool<ResourceLoadRequest>(DefaultRequestPoolSize);
            
            this.history = new Dictionary<ResourceKey, long>();

            this.fallbackResources = new Dictionary<Type, ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event OnResourceLoadingDelegate ResourceLoading;
        public event OnResourceLoadedDelegate ResourceLoaded;

        public int PendingForLoad
        {
            get
            {
                return this.currentPendingLoads.Count;
            }
        }

        public int ResourcesLoaded { get; private set; }

        public bool EnableHistory { get; set; }

        public bool EnableInstantiation { get; set; }

        public ResourceRequestPool<ResourceLoadRequest> RequestPool
        {
            get
            {
                return this.requestPool;
            }
        }

        public static T SingletonResource<T>()
            where T : UnityEngine.Object
        {
            IList<ResourceKey> resources = Instance.AcquireResourcesByType<T>();
            if (resources == null || resources.Count != 1)
            {
                Diagnostic.Warning("Expected 1 result for {0}", typeof(T));
                return null;
            }

            return Instance.AcquireResource<T>(resources.First()).Data;
        }

        public static UnityEngine.Object LoadImmediate(ResourceKey key)
        {
            if (key.Bundle != null)
            {
                AssetBundle bundle = BundleProvider.Instance.GetBundle(key.Bundle.Value);

                return bundle.LoadAsset(key.Path, key.Type ?? typeof(UnityEngine.Object));
            }

            return Resources.Load(key.Path, key.Type ?? typeof(UnityEngine.Object));
        }

        // Note: Use this only when we can not do an async loading, avoid if possible
        public static T LoadImmediate<T>(ResourceKey key)
            where T : UnityEngine.Object
        {
            return LoadImmediate(key) as T;
        }

        public IList<ResourceKey> AcquireResourcesByType<T>()
        {
            return this.resourceMap.GetKeysByType<T>();
        }

        public IDictionary<ResourceKey, long> GetHistory()
        {
            return this.history;
        }

        public void RegisterLoadedResource(ResourceKey key, UnityEngine.Object resource)
        {
            Diagnostic.Assert(resource != null, "Registering a loaded resource with null data!");

            // Register the resource without queuing
            this.resourceMap.RegisterResource(key, resource);
        }

        public void RegisterResource(ResourceKey key, ResourceLoadFlags flags = ResourceLoadFlags.Cache)
        {
            if ((flags & ResourceLoadFlags.Cache) != 0)
            {
                // Cache the resource in the map
                this.resourceMap.RegisterResource(key);
            }

            lock (this.currentPendingLoads)
            {
                if (!this.EnableInstantiation)
                {
                    flags &= ~ResourceLoadFlags.Instantiate;
                }

                this.currentPendingLoads.Enqueue(new ResourceLoadInfo(key, flags));
            }
        }

        public void RegisterFallbackResource(ResourceKey key, ResourceLoadFlags flags = ResourceLoadFlags.Cache)
        {
            if (this.fallbackResources.ContainsKey(key.Type))
            {
                Diagnostic.Warning("Duplicate fallback resource registered for type {0}", key.Type);
                return;
            }

            this.RegisterResource(key, flags);
            this.fallbackResources.Add(key.Type, key);
        }

        public void UnregisterResource(ResourceKey key)
        {
            this.resourceMap.UnregisterResource(key);
        }

        public ResourceReference<T> AcquireOrLoadResource<T>(ResourceKey key, ResourceLoadFlags flags = ResourceLoadFlags.Cache)
            where T : UnityEngine.Object
        {
            UnityEngine.Object data = this.resourceMap.GetData(key);
            if (data == null)
            {
                if (key.Bundle != null)
                {
                    BundleProvider.Instance.LoadBundleImmediate(key.Bundle.Value);
                }

                this.DoLoadImmediate(new ResourceLoadInfo(key, flags));
                data = this.resourceMap.GetData(key);
                if (data == null)
                {
                    Diagnostic.Error("Could not load resource on-demand");
                    return null;
                }
            }

            return this.BuildReference<T>(key, data);
        }

        public ResourceReference<T> AcquireResource<T>(ResourceKey key)
            where T : UnityEngine.Object
        {
            UnityEngine.Object data = this.resourceMap.GetData(key);
            if (data == null)
            {
                data = this.AcquireFallbackResource<T>();
                if (data == null)
                {
                    Diagnostic.Error("Resource was not loaded or registered: {0}", key);
                    return null;
                }
            }

            return this.BuildReference<T>(key, data);
        }

        public bool TryAcquireResource<T>(ResourceKey key, out ResourceReference<T> reference)
            where T : UnityEngine.Object
        {
            reference = null;
            UnityEngine.Object data = this.resourceMap.GetData(key);
            if (data == null)
            {
                return false;
            }

            reference = this.BuildReference<T>(key, data);
            return reference != null;
        }
        
        public void ReleaseResource<T>(ResourceReference<T> reference)
            where T : UnityEngine.Object
        {
            this.DecreaseResourceRefCount(reference.Key);
        }

        public bool ContinueLoad()
        {
            IList<ResourceLoadRequest> finishedRequests = this.requestPool.GetFinishedRequests();
            if (finishedRequests != null)
            {
                foreach (ResourceLoadRequest request in finishedRequests)
                {
                    Diagnostic.TakeTimeMeasure(request.Metric);

                    UnityEngine.Object[] results = request.GetAssets();
                    if (results != null && results.Length == 1)
                    {
                        this.FinalizeLoadResource(request.Info, results[0], request.Metric);
                    }
                    else
                    {
                        Diagnostic.Warning("Load of {0} returned unexpected result, got {1}", request, results);
                    }
                }
            }

            int consecutiveSyncCalls = 0;
            while (this.currentPendingLoads.Count > 0 && this.requestPool.HasFreeSlot())
            {
                ResourceLoadInfo info = this.currentPendingLoads.Dequeue();

                if (this.resourceMap.HasData(info.Key))
                {
                    // This resource was already loaded, continue
                    return true;
                }

                if ((info.Flags & ResourceLoadFlags.Sync) != 0)
                {
                    // This resource is a forced sync load
                    this.DoLoadImmediate(info);
                    consecutiveSyncCalls++;

                    if (consecutiveSyncCalls > MaxConsecutiveSyncCallsInAsync)
                    {
                        // Give a frame for the UI to catch up, we probably load a lot of tiny resources
                        return true;
                    }

                    continue;
                }

                if (this.ResourceLoading != null)
                {
                    this.ResourceLoading(info);
                }

                this.requestPool.AddRequest(DoLoad(info));
            }

            this.CleanupPendingInstantiations();

            return this.currentPendingLoads.Count > 0 || this.requestPool.HasPendingRequests() || this.pendingInstantiations.Count > 0;
        }

        public void LoadImmediate()
        {
            if (this.currentPendingLoads.Count <= 0)
            {
                return;
            }

            MetricTime totalTime = Diagnostic.BeginTimeMeasure();
            int resourceCount = this.currentPendingLoads.Count;
            while (this.currentPendingLoads.Count > 0)
            {
                ResourceLoadInfo info = this.currentPendingLoads.Dequeue();
                if (!this.resourceMap.HasData(info.Key))
                {
                    this.DoLoadImmediate(info);
                }
            }
            
            Diagnostic.TakeTimeMeasure(totalTime);
            Diagnostic.Info("Immediate! Loaded {0} resources in {1}ms", resourceCount, Diagnostic.GetTimeInMS(totalTime.Total));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static ResourceLoadRequest DoLoad(ResourceLoadInfo info)
        {
            if (info.Key.Bundle != null)
            {
                AssetBundle bundle = BundleProvider.Instance.GetBundle(info.Key.Bundle.Value);

                AssetBundleRequest request = bundle.LoadAssetAsync(info.Key.Path, info.Key.Type);
                return new ResourceLoadRequest(info, request);
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync(info.Key.Path, info.Key.Type);
                return new ResourceLoadRequest(info, request);
            }
        }

        private ResourceReference<T> BuildReference<T>(ResourceKey key, UnityEngine.Object data)
            where T : UnityEngine.Object
        {
            if (!(data is T))
            {
                Diagnostic.Error("Type requested {0} did not match the registered key type {1} for {2}", typeof(T), key.Type, key);
                return null;
            }

            var reference = new ResourceReference<T>(key, (T)data, this);
            this.IncreaseResourceRefCount(key);
            return reference;
        }

        private void DoLoadImmediate(ResourceLoadInfo info)
        {
            if (this.ResourceLoading != null)
            {
                this.ResourceLoading(info);
            }

            MetricTime resourceTime = Diagnostic.BeginTimeMeasure();
            UnityEngine.Object result = LoadImmediate(info.Key);
            Diagnostic.TakeTimeMeasure(resourceTime);

            this.FinalizeLoadResource(info, result, resourceTime);
        }

        private void IncreaseResourceRefCount(ResourceKey key)
        {
            if (!this.referenceCount.ContainsKey(key))
            {
                this.referenceCount.Add(key, 0);
            }

            this.referenceCount[key]++;
        }

        private void DecreaseResourceRefCount(ResourceKey key)
        {
            if (!this.referenceCount.ContainsKey(key))
            {
                return;
            }

            this.referenceCount[key]--;
            if (this.referenceCount[key] <= 0)
            {
                this.referenceCount.Remove(key);
            }
        }

        private void FinalizeLoadResource(ResourceLoadInfo info, UnityEngine.Object data, MetricTime elapsedTime)
        {
            if (data == null)
            {
                Diagnostic.Warning("Loading {0} returned null data", info.Key);
                return;
            }

            if (this.EnableHistory)
            {
                if (this.history.ContainsKey(info.Key))
                {
                    this.history[info.Key] += elapsedTime.Total;
                }
                else
                {
                    this.history.Add(info.Key, elapsedTime.Total);
                }
            }

            if ((info.Flags & ResourceLoadFlags.Instantiate) != 0 && data is GameObject)
            {
                try
                {
                    var instance = UnityEngine.Object.Instantiate(data) as GameObject;
                    this.pendingInstantiations.Add(instance);
                }
                catch (Exception e)
                {
                    Diagnostic.Error("Failed to instantiate resource {0} on load: {1}", info.Key, e);
                }
            }

            this.resourceMap.SetData(info.Key, data);

            this.ResourcesLoaded++;
            if (this.ResourceLoaded != null)
            {
                this.ResourceLoaded(info, elapsedTime.Total);
            }
        }

        private void CleanupPendingInstantiations()
        {
            foreach (UnityEngine.Object gameObject in this.pendingInstantiations)
            {
                UnityEngine.Object.Destroy(gameObject);
            }

            this.pendingInstantiations.Clear();
        }

        private UnityEngine.Object AcquireFallbackResource<T>()
        {
            ResourceKey fallbackKey;
            if (this.fallbackResources.TryGetValue(typeof(T), out fallbackKey))
            {
                return this.resourceMap.GetData(fallbackKey);
            }

            return null;
        }
    }
}
