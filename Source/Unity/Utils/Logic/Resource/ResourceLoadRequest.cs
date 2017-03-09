namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using System;

    using CarbonCore.Unity.Utils.Contracts;
    using CarbonCore.Unity.Utils.Logic.Enums;

    using UnityEngine;

    public class ResourceLoadRequest : IResourceRequest
    {
        private readonly UnityEngine.Object asset;
        private readonly ResourceRequest resourceRequest;
        private readonly AssetBundleRequest bundleRequest;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadRequest(ResourceLoadInfo info, ResourceRequest internalRequest)
            : this(info)
        {
            this.resourceRequest = internalRequest;
            this.Mode = ResourceLoadMode.Internal;
        }

        public ResourceLoadRequest(ResourceLoadInfo info, AssetBundleRequest internalRequest)
            : this(info)
        {
            this.bundleRequest = internalRequest;
            this.Mode = ResourceLoadMode.Bundle;
        }

        public ResourceLoadRequest(ResourceLoadInfo info, UnityEngine.Object asset)
            : this(info)
        {
            this.asset = asset;
            this.Mode = ResourceLoadMode.Assigned;
        }

        protected ResourceLoadRequest(ResourceLoadInfo info)
        {
            this.Info = info;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceLoadInfo Info { get; private set; }

        public ResourceLoadMode Mode { get; private set; }
        
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
                        return this.asset as T;
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

        public UnityEngine.Object GetAsset()
        {
            switch (this.Mode)
            {
                    case ResourceLoadMode.Assigned:
                    {
                        return this.asset;
                    }

                    case ResourceLoadMode.Internal:
                    {
                        return this.resourceRequest.asset;
                    }

                    case ResourceLoadMode.Bundle:
                    {
                        return this.bundleRequest.asset;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
