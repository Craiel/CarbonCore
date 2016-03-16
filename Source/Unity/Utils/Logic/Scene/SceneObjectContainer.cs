namespace CarbonCore.Unity.Utils.Logic.Scene
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Logic.Scene;

    using UnityEngine;

    public class SceneObjectContainer
    {
        private readonly IDictionary<string, SceneObjectRoot> rootEntries;

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        public SceneObjectContainer(SceneObjectContainer parent, string name)
        {
            this.Name = name;

            this.GameObject = new GameObject(name);
            Object.DontDestroyOnLoad(this.GameObject);

            if (parent != null)
            {
                this.GameObject.transform.SetParent(parent.GameObject.transform);
            }

            this.rootEntries = new Dictionary<string, SceneObjectRoot>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public GameObject GameObject { get; private set; }
        
        public int ChildCount
        {
            get
            {
                return this.rootEntries.Count;
            }
        }

        public void Clear(bool clearPersistentObjects)
        {
            foreach (string key in new List<string>(this.rootEntries.Keys))
            {
                SceneObjectRoot root = this.rootEntries[key];
                if (root.Persistent && !clearPersistentObjects)
                {
                    continue;
                }

                root.Destroy();

                rootEntries.Remove(key);
            }
        }

        public SceneObjectRoot AcquireRoot(string name)
        {
            string key = name.ToLowerInvariant();

            SceneObjectRoot root;
            if (!rootEntries.TryGetValue(key, out root) ||
                root.GameObject == null)
            {
                root = new SceneObjectRoot(this, name);
                this.rootEntries[key] = root;
            }

            return root;
        }

        public SceneObjectRoot RegisterAsRoot(GameObject gameObject)
        {
            string key = gameObject.name.ToLowerInvariant();

            if (this.rootEntries.ContainsKey(key))
            {
                Diagnostic.Error("Root with same key is already registered: {0}", key);
                return null;
            }

            this.rootEntries.Add(key, new SceneObjectRoot(this, gameObject));
            return this.rootEntries[key];
        }
    }
}
