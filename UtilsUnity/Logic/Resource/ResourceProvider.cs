namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Data;
    using CarbonCore.Utils.Unity.Logic;

    public delegate void OnResourceLoadingDelegate(ResourceKey key);
    public delegate void OnResourceLoadedDelegate(ResourceKey key, long loadTime);

    public class ResourceProvider : UnitySingleton<ResourceProvider>
    {
        private const int DefaultRequestPoolSize = 30;

        private const int MaxConsecutiveSyncCallsInAsync = 20;

        private readonly ResourceMap resourceMap;

        private readonly IDictionary<ResourceKey, int> referenceCount;
        
        private readonly Queue<ResourceKey> currentPendingLoads;
        private readonly IList<ResourceKey> instantiateOnLoad;
        private readonly IList<ResourceKey> forceSyncLoad;
        private readonly IList<UnityEngine.Object> pendingInstantiations;

        private readonly ResourceLoadRequestPool requestPool;

#if UNITY_EDITOR
        private readonly IDictionary<ResourceKey, long> history;
#endif
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceProvider()
        {
            this.resourceMap = new ResourceMap();
            this.referenceCount = new Dictionary<ResourceKey, int>();

            this.currentPendingLoads = new Queue<ResourceKey>();
            this.instantiateOnLoad = new List<ResourceKey>();
            this.forceSyncLoad = new List<ResourceKey>();
            this.pendingInstantiations = new List<UnityEngine.Object>();

            this.requestPool = new ResourceLoadRequestPool(DefaultRequestPoolSize);

#if UNITY_EDITOR
            this.history = new Dictionary<ResourceKey, long>();
#endif
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

        public bool EnableInstantiation { get; set; }

        public ResourceLoadRequestPool RequestPool
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

        public IList<ResourceKey> AcquireResourcesByType<T>()
        {
            return this.resourceMap.GetKeysByType<T>();
        }

        public IDictionary<ResourceKey, long> GetHistory()
        {
#if UNITY_EDITOR
            return this.history;
#else
            Diagnostic.Warning("Resource History is not available in Release build!");
            return null;
#endif
        }

        public void RegisterLoadedResource(ResourceKey key, UnityEngine.Object resource)
        {
            Diagnostic.Assert(resource != null, "Registering a loaded resource with null data!");

            // Register the resource without queuing
            this.resourceMap.RegisterResource(key, resource);
        }

        public void RegisterResource(ResourceKey key, bool instantiateOnload = false, bool forceSyncLoad = false)
        {
            this.resourceMap.RegisterResource(key);

            lock (this.currentPendingLoads)
            {
                this.currentPendingLoads.Enqueue(key);

                if (forceSyncLoad)
                {
                    this.forceSyncLoad.Add(key);
                }

                if (this.EnableInstantiation && instantiateOnload)
                {
                    this.instantiateOnLoad.Add(key);
                }
            }
        }

        public void UnregisterResource(ResourceKey key)
        {
            this.resourceMap.UnregisterResource(key);

#if UNITY_EDITOR
            if (this.history.ContainsKey(key))
            {
                this.history.Remove(key);
            }
#endif
        }

        public ResourceReference<T> AcquireOrLoadResource<T>(ResourceKey key)
            where T : UnityEngine.Object
        {
            UnityEngine.Object data = this.resourceMap.GetData(key);
            if (data == null)
            {
                if (key.Bundle != null)
                {
                    BundleProvider.Instance.LoadBundleImmediate(key.Bundle.Value);
                }

                this.DoLoadImmediate(key);
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
                Diagnostic.Error("Resource was not loaded or registered: {0}", key);
                return null;
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
                        this.FinalizeLoadResource(request.Key, results[0], request.Metric);
                    }
                    else
                    {
                        Diagnostic.Warning("Load of {0} returned unexpected result, got {1}", request.Key, results);
                    }
                }
            }

            int consecutiveSyncCalls = 0;
            while (this.currentPendingLoads.Count > 0 && this.requestPool.HasFreeSlot())
            {
                ResourceKey key = this.currentPendingLoads.Dequeue();

                if (this.resourceMap.HasData(key))
                {
                    // This resource was already loaded, continue
                    return true;
                }

                if (this.forceSyncLoad.Contains(key))
                {
                    // This resource is a forced sync load
                    this.DoLoadImmediate(key);
                    this.forceSyncLoad.Remove(key);
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
                    this.ResourceLoading(key);
                }

                this.requestPool.AddRequest(ResourceLoader.Load(key));
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
                ResourceKey key = this.currentPendingLoads.Dequeue();
                if (!this.resourceMap.HasData(key))
                {
                    this.DoLoadImmediate(key);
                }
            }

            this.forceSyncLoad.Clear();

            Diagnostic.TakeTimeMeasure(totalTime);
            Diagnostic.Info("Immediate! Loaded {0} resources in {1}ms", resourceCount, Diagnostic.GetTimeInMS(totalTime.Total));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ResourceReference<T> BuildReference<T>(ResourceKey key, UnityEngine.Object data)
            where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!(data is T))
            {
                Diagnostic.Error("Type requested {0} did not match the registered key type {1} for {2}", typeof(T), key.Type, key);
                return null;
            }
#endif

            var reference = new ResourceReference<T>(key, (T)data, this);
            this.IncreaseResourceRefCount(key);
            return reference;
        }

        private void DoLoadImmediate(ResourceKey key)
        {
            if (this.ResourceLoading != null)
            {
                this.ResourceLoading(key);
            }

            MetricTime resourceTime = Diagnostic.BeginTimeMeasure();
            UnityEngine.Object result = ResourceLoader.LoadImmediate(key);
            Diagnostic.TakeTimeMeasure(resourceTime);

            this.FinalizeLoadResource(key, result, resourceTime);
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

        private void FinalizeLoadResource(ResourceKey key, UnityEngine.Object data, MetricTime elapsedTime)
        {
            if (data == null)
            {
                Diagnostic.Warning("Loading {0} returned null data", key);
                return;
            }

#if UNITY_EDITOR
            if (this.history.ContainsKey(key))
            {
                this.history[key] += elapsedTime.Total;
            }
            else
            {
                this.history.Add(key, elapsedTime.Total);
            }
#endif

            if (this.EnableInstantiation && this.instantiateOnLoad.Contains(key) && data is UnityEngine.GameObject)
            {
                try
                {
                    var instance = UnityEngine.Object.Instantiate(data) as UnityEngine.GameObject;
                    this.pendingInstantiations.Add(instance);
                }
                catch (Exception e)
                {
                    Diagnostic.Error("Failed to instantiate resource {0} on load: {1}", key, e);
                }
                
                this.instantiateOnLoad.Remove(key);
            }

            this.resourceMap.SetData(key, data);

            this.ResourcesLoaded++;
            if (this.ResourceLoaded != null)
            {
                this.ResourceLoaded(key, elapsedTime.Total);
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
    }
}
