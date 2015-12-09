﻿namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Unity.Data;

    public class ResourceMap<T> where T : class
    {
        private const int KeyLookupLength = 2;

        private readonly IDictionary<ResourceKey, T> data;

        private readonly IDictionary<string, IList<ResourceKey>> keyLookup;
        private readonly IDictionary<Type, IList<ResourceKey>> typeLookup;

        private readonly IList<ResourceKey> resources;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceMap()
        {
            this.data = new Dictionary<ResourceKey, T>();
            this.keyLookup = new Dictionary<string, IList<ResourceKey>>();
            this.typeLookup = new Dictionary<Type, IList<ResourceKey>>();
            this.resources = new List<ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<ResourceKey> GetKeysByType<T>()
        {
            if (this.typeLookup.ContainsKey(typeof(T)))
            {
                return this.typeLookup[typeof(T)];
            }

            return null;
        }

        public void RegisterResource(ResourceKey key, T resourceData = null)
        {
            if (this.resources.Contains(key))
            {
                // Resource is already registered
                return;
            }

            this.resources.Add(key);

            // Register the string lookup
            string lookupKey = key.Path.Substring(0, KeyLookupLength).ToLowerInvariant();
            if (!this.keyLookup.ContainsKey(lookupKey))
            {
                this.keyLookup.Add(lookupKey, new List<ResourceKey>());
            }

            this.keyLookup[lookupKey].Add(key);

            // Register the type lookup
            if (!this.typeLookup.ContainsKey(key.Type))
            {
                this.typeLookup.Add(key.Type, new List<ResourceKey>());
            }

            this.typeLookup[key.Type].Add(key);

            if (resourceData != null)
            {
                this.SetData(key, resourceData);
            }
        }

        public void UnregisterResource(ResourceKey key)
        {
            // Unregister the string lookup
            string lookupKey = key.Path.Substring(0, KeyLookupLength).ToLowerInvariant();
            this.keyLookup[lookupKey].Remove(key);

            // Unregister the type lookup
            this.typeLookup[key.Type].Remove(key);

            if (this.data.ContainsKey(key))
            {
                this.data.Remove(key);
            }

            this.resources.Remove(key);
        }

        public T GetData(ResourceKey key)
        {
            T result;
            if (this.data.TryGetValue(key, out result))
            {
                return result;
            }

            return null;
        }

        public bool HasData(ResourceKey key)
        {
            return this.data.ContainsKey(key) && this.data[key] != null;
        }

        public void SetData(ResourceKey key, T resourceData)
        {
            if (this.data.ContainsKey(key))
            {
                this.data[key] = resourceData;
            }
            else
            {
                this.data.Add(key, resourceData);
            }
        }
    }
}