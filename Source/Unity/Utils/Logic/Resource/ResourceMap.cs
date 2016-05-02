namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Unity.Utils.Data;
    using CarbonCore.Utils.Diagnostics;

    public class ResourceMap<T> where T : class
    {
        private const int KeyLookupLength = 2;
        private const int KeyLookupMaxDepth = 50;

        private readonly IDictionary<ResourceKey, T> data;

        private readonly IDictionary<string, IList<ResourceKey>> keyLookup;
        private readonly IDictionary<Type, IList<ResourceKey>> typeLookup;

        private readonly IDictionary<ResourceKey, ResourceKey> linkToResource;
        private readonly IDictionary<ResourceKey, IList<ResourceKey>> linkedTo;

        private readonly HashSet<ResourceKey> resources;
        private readonly HashSet<ResourceKey> links;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceMap()
        {
            this.data = new Dictionary<ResourceKey, T>();

            this.keyLookup = new Dictionary<string, IList<ResourceKey>>();
            this.typeLookup = new Dictionary<Type, IList<ResourceKey>>();

            this.linkToResource = new Dictionary<ResourceKey, ResourceKey>();
            this.linkedTo = new Dictionary<ResourceKey, IList<ResourceKey>>();

            this.links = new HashSet<ResourceKey>();
            this.resources = new HashSet<ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<ResourceKey> GetKeysByType<TX>()
        {
            return this.GetKeysByType(typeof(TX));
        }

        public IList<ResourceKey> GetKeysByType(Type type)
        {
            if (this.typeLookup.ContainsKey(type))
            {
                return this.typeLookup[type];
            }

            return null;
        }

        public void RegisterLink(ResourceKey source, ResourceKey target, bool overwriteExisting = false)
        {
            if (this.resources.Contains(source))
            {
                Diagnostic.Error("Resource {0} was already loaded, can not add as a link");
                return;
            }

            if (this.links.Contains(source))
            {
                if (overwriteExisting)
                {
                    this.UnregisterLink(source);
                }
                else
                {
                    Diagnostic.Error("Resource {0} was already linked to {1}, skipping link to {2}", source, this.linkToResource[source], target);
                    return;
                }
            }

            this.links.Add(source);
            this.linkToResource.Add(source, target);
            IList<ResourceKey> linkedToList;
            if (!this.linkedTo.TryGetValue(target, out linkedToList))
            {
                linkedToList = new List<ResourceKey>();
                this.linkedTo.Add(target, linkedToList);
            }

            linkedToList.Add(source);
        }

        public void UnregisterLink(ResourceKey source)
        {
            if (!this.links.Contains(source))
            {
                // Link was not registered
                Diagnostic.Warning("Unregister link called with non existing key {0}", source);
                return;
            }

            // Unregister the link in all required lists
            this.links.Remove(source);
            this.linkedTo[this.linkToResource[source]].Remove(source);
            this.linkToResource.Remove(source);
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
            IList<ResourceKey> keyList;
            if (!this.keyLookup.TryGetValue(lookupKey, out keyList))
            {
                keyList = new List<ResourceKey>();
                this.keyLookup.Add(lookupKey, keyList);
            }

            keyList.Add(key);

            // Register the type lookup
            IList<ResourceKey> typeList;
            if (!this.typeLookup.TryGetValue(key.Type, out typeList))
            {
                typeList = new List<ResourceKey>();
                this.typeLookup.Add(key.Type, typeList);
            }

            typeList.Add(key);

            this.data.Add(key, resourceData);
        }

        public void UnregisterResource(ResourceKey key, bool cleanupLinksToResource = false)
        {
            if (!this.data.ContainsKey(key))
            {
                Diagnostic.Warning("Unregister Resource called with non existing key {0}", key);
                return;
            }

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

            if (cleanupLinksToResource)
            {
                IList<ResourceKey> linksToResource;
                if (this.linkedTo.TryGetValue(key, out linksToResource))
                {
                    foreach (ResourceKey linkKey in linksToResource)
                    {
                        this.links.Remove(linkKey);
                    }

                    this.linkedTo.Remove(key);
                }
            }
        }

        public T GetData(ResourceKey key)
        {
            ResourceKey? existingKey = this.FindExistingResourceKey(key);
            if (existingKey != null)
            {
                return this.data[existingKey.Value];
            }
            
            return null;
        }

        public bool HasData(ResourceKey key)
        {
            ResourceKey? existingKey = this.FindExistingResourceKey(key);
            return existingKey != null && this.data[existingKey.Value] != null;
        }

        public void SetData(ResourceKey key, T resourceData)
        {
            ResourceKey? existingKey = this.FindExistingResourceKey(key);
            if (existingKey != null)
            {
                this.data[existingKey.Value] = resourceData;
                return;
            }

            this.RegisterResource(key, resourceData);
        }

        public ResourceKey? GetLinkTarget(ResourceKey source)
        {
            ResourceKey result;
            if (this.linkToResource.TryGetValue(source, out result))
            {
                return result;
            }

            return null;
        }

        public IList<ResourceKey> GetLinkedTo(ResourceKey target)
        {
            IList<ResourceKey> result;
            if (this.linkedTo.TryGetValue(target, out result))
            {
                return result;
            }

            return null;
        }

        private ResourceKey? FindExistingResourceKey(ResourceKey keyIn)
        {
            int depth = 0;
            return this.FindExistingResourceKey(keyIn, ref depth);
        }

        private ResourceKey? FindExistingResourceKey(ResourceKey keyIn, ref int depth)
        {
            if (depth > KeyLookupMaxDepth)
            {
                Diagnostic.Error("ResourceKey lookup exceeded max depth, probably circular resource link!");
                return null;
            }

            if (this.resources.Contains(keyIn))
            {
                return keyIn;
            }

            // now try to follow the links if there are any
            ResourceKey linkTarget;
            if (this.linkToResource.TryGetValue(keyIn, out linkTarget))
            {
                depth++;
                return this.FindExistingResourceKey(linkTarget, ref depth);
            }

            return null;
        }
    }
}
