namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System;
    using System.Linq;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Data;
    using CarbonCore.Utils.Unity.Logic.Enums;

    using UnityEngine;

    public class ResourceLoadRequest
    {
        private readonly UnityEngine.Object[] assets;
        private readonly ResourceRequest resourceRequest;
        private readonly AssetBundleRequest bundleRequest;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadRequest(ResourceKey key, ResourceRequest internalRequest)
            : this(key)
        {
            this.resourceRequest = internalRequest;
            this.Mode = ResourceLoadMode.Internal;
        }

        public ResourceLoadRequest(ResourceKey key, AssetBundleRequest internalRequest)
            : this(key)
        {
            this.bundleRequest = internalRequest;
            this.Mode = ResourceLoadMode.Bundle;
        }

        public ResourceLoadRequest(ResourceKey key, UnityEngine.Object[] assets)
            : this(key)
        {
            this.assets = assets;
            this.Mode = ResourceLoadMode.Assigned;
        }

        protected ResourceLoadRequest(ResourceKey key)
        {
            this.Key = key;

            this.Metric = Diagnostic.BeginTimeMeasure();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceKey Key { get; private set; }

        public ResourceLoadMode Mode { get; private set; }

        public MetricTime Metric { get; private set; }

        public bool IsDone
        {
            get
            {
                switch (this.Mode)
                {
                    case ResourceLoadMode.Assigned:
                        {
                            return true;
                        }

                    case ResourceLoadMode.Bundle:
                        {
                            return this.bundleRequest.isDone;
                        }

                    case ResourceLoadMode.Internal:
                        {
                            return this.resourceRequest.isDone;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            switch (this.Mode)
            {
                case ResourceLoadMode.Assigned:
                    {
                        return this.assets.FirstOrDefault() as T;
                    }

                case ResourceLoadMode.Internal:
                    {
                        return this.resourceRequest.asset as T;
                    }

                case ResourceLoadMode.Bundle:
                    {
                        return this.bundleRequest.asset as T;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        public UnityEngine.Object[] GetAssets()
        {
            switch (this.Mode)
            {
                    case ResourceLoadMode.Assigned:
                    {
                        return this.assets;
                    }

                    case ResourceLoadMode.Internal:
                    {
                        return new[] { this.resourceRequest.asset };
                    }

                    case ResourceLoadMode.Bundle:
                    {
                        return this.bundleRequest.allAssets;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
