namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System.Collections.Generic;
    using System.Threading;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Data;

    // Made to load resources from Application.streamingAssetsPath and other WWW accessible places
    public class ResourceStreamProvider : UnitySingleton<ResourceStreamProvider>
    {
        private const int DefaultRequestPoolSize = 30;

        private const float DefaultReadTimeout = 10;

        private const int MaxConsecutiveSyncCallsInAsync = 20;

        private readonly ResourceMap<byte[]> resourceMap;

        private readonly Queue<ResourceKey> currentPendingLoads;
        private readonly IList<ResourceKey> forceSyncLoad;

        private readonly ResourceRequestPool<ResourceStreamRequest> requestPool;

        private readonly IDictionary<ResourceKey, long> history;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamProvider()
        {
            this.resourceMap = new ResourceMap<byte[]>();

            this.currentPendingLoads = new Queue<ResourceKey>();
            this.forceSyncLoad = new List<ResourceKey>();

            this.requestPool = new ResourceRequestPool<ResourceStreamRequest>(DefaultRequestPoolSize);

            this.history = new Dictionary<ResourceKey, long>();
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

        public IDictionary<ResourceKey, long> GetHistory()
        {
            return this.history;
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
            }
        }

        public void UnregisterResource(ResourceKey key)
        {
            this.resourceMap.UnregisterResource(key);
        }

        public byte[] AcquireOrLoadResource(ResourceKey key)
        {
            byte[] data = this.resourceMap.GetData(key);
            if (data == null)
            {
                if (key.Bundle != null)
                {
                    Diagnostic.Error("Can not stream bundled resources!");
                    return null;
                }

                this.DoLoadImmediate(key);
                data = this.resourceMap.GetData(key);
                if (data == null)
                {
                    Diagnostic.Error("Could not load resource on-demand");
                    return null;
                }
            }

            return data;
        }

        public byte[] AcquireResource(ResourceKey key)
        {
            byte[] data = this.resourceMap.GetData(key);
            if (data == null)
            {
                Diagnostic.Error("Resource was not loaded or registered: {0}", key);
                return null;
            }

            return data;
        }

        public string AcquireStringResource(ResourceKey key)
        {
            byte[] data = this.AcquireResource(key);
            if (data == null)
            {
                return null;
            }
            
            if (this.TextContainsBOM(data))
            {
                return System.Text.Encoding.UTF8.GetString(data, 3, data.Length - 3);
            }

            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public bool TryAcquireResource(ResourceKey key, out byte[] data)
        {
            data = this.resourceMap.GetData(key);
            return data != null;
        }
        
        public void ClearCache(ResourceKey key)
        {
            if (this.resourceMap.HasData(key))
            {
                this.resourceMap.SetData(key, null);
            }
        }

        public bool ContinueLoad()
        {
            IList<ResourceStreamRequest> finishedRequests = this.requestPool.GetFinishedRequests();
            if (finishedRequests != null)
            {
                foreach (ResourceStreamRequest request in finishedRequests)
                {
                    Diagnostic.TakeTimeMeasure(request.Metric);

                    byte[] result = request.GetData();
                    if (result != null)
                    {
                        this.FinalizeLoadResource(request.Key, result, request.Metric);
                    }
                    else
                    {
                        Diagnostic.Warning("Load of {0} returned unexpected no data", request.Key);
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

                this.requestPool.AddRequest(new ResourceStreamRequest(key));
            }
            
            return this.currentPendingLoads.Count > 0 || this.requestPool.HasPendingRequests();
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
        private void DoLoadImmediate(ResourceKey key)
        {
            if (this.ResourceLoading != null)
            {
                this.ResourceLoading(key);
            }

            MetricTime resourceTime = Diagnostic.BeginTimeMeasure();
            var request = new ResourceStreamRequest(key);
            float time = UnityEngine.Time.time;
            while (!request.IsDone)
            {
                Thread.Sleep(2);
                if (UnityEngine.Time.time > time + DefaultReadTimeout)
                {
                    Diagnostic.Error("Timeout while reading {0}", key);
                    return;
                }
            }
            
            Diagnostic.TakeTimeMeasure(resourceTime);

            this.FinalizeLoadResource(key, request.GetData(), resourceTime);
        }
        
        private void FinalizeLoadResource(ResourceKey key, byte[] data, MetricTime elapsedTime)
        {
            if (data == null)
            {
                Diagnostic.Warning("Loading {0} returned null data", key);
                return;
            }

            if (this.EnableHistory)
            {
                if (this.history.ContainsKey(key))
                {
                    this.history[key] += elapsedTime.Total;
                }
                else
                {
                    this.history.Add(key, elapsedTime.Total);
                }
            }
            
            this.resourceMap.SetData(key, data);

            this.ResourcesLoaded++;
            if (this.ResourceLoaded != null)
            {
                this.ResourceLoaded(key, elapsedTime.Total);
            }
        }

        private bool TextContainsBOM(byte[] textData)
        {
            // Getting text through WWW will sometimes return the data with BOM
            // 239 187 191 (EF BB BF)
            if (textData.Length < 3)
            {
                return false;
            }

            return textData[0] == 239 && textData[1] == 187 && textData[2] == 191;
        }
    }
}
