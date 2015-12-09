namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Data;

    using UnityEngine;

    public static class ResourceLoader
    {
        private static readonly IDictionary<ResourceKey, UnityEngine.Object> Cache;

        private static readonly IDictionary<BundleKey, AssetBundle> Bundles;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static ResourceLoader()
        {
            Cache = new Dictionary<ResourceKey, UnityEngine.Object>();
            Bundles = new Dictionary<BundleKey, AssetBundle>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void RegisterBundle(BundleKey key, AssetBundle bundle)
        {
            if (Bundles.ContainsKey(key))
            {
                // Todo: refactor with resource loading
                Diagnostic.Warning("Bundle {0} already loaded!", bundle.name);
                return;
            }

            Bundles.Add(key, bundle);
        }

        public static void UnregisterBundle(BundleKey key)
        {
            Diagnostic.Assert(Bundles.ContainsKey(key));

            Bundles.Remove(key);
        }

        public static void RegisterAssetCache(ResourceKey key, UnityEngine.Object asset)
        {
            if (Cache.ContainsKey(key))
            {
                Cache[key] = asset;
            }
            else
            {
                Cache.Add(key, asset);
            }
        }

        public static ResourceLoadRequest Load(ResourceKey key, bool useCache = true, bool allowCaching = false)
        {
            if (useCache && Cache.ContainsKey(key))
            {
                return new ResourceLoadRequest(key, new[] { Cache[key] });
            }

            if (key.Bundle != null)
            {
                AssetBundle bundle = Bundles[key.Bundle.Value];

                AssetBundleRequest request = bundle.LoadAssetAsync(key.Path, key.Type);
                return new ResourceLoadRequest(key, request);
            }
            else
            {
                ResourceRequest request = Resources.LoadAsync(key.Path, key.Type);
                return new ResourceLoadRequest(key, request);
            }
        }

        public static UnityEngine.Object LoadImmediate(ResourceKey key, bool useCache = true)
        {
            if (useCache && Cache.ContainsKey(key))
            {
                return Cache[key];
            }

            if (key.Bundle != null)
            {
                AssetBundle bundle = Bundles[key.Bundle.Value];

                return bundle.LoadAsset(key.Path, key.Type ?? typeof(UnityEngine.Object));
            }

            return Resources.Load(key.Path, key.Type ?? typeof(UnityEngine.Object));
        }

        // Note: Use this only when we can not do an async loading, avoid if possible
        public static T LoadImmediate<T>(ResourceKey key, bool useCache = true)
            where T : UnityEngine.Object
        {
            return LoadImmediate(key, useCache) as T;
        }

        public static void ClearCache()
        {
            Cache.Clear();
        }
    }
}
