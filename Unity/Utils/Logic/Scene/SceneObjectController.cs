namespace CarbonCore.Utils.Unity.Logic.Scene
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    
    using UnityEngine;

    public class SceneObjectController<T> : UnitySingleton<SceneObjectController<T>>
        where T : struct, IConvertible
    {
        private const string RootName = "Scene";

        private readonly IDictionary<T, SceneObjectContainer> containers;

        private SceneObjectContainer rootContainer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectController()
        {
            this.containers = new Dictionary<T, SceneObjectContainer>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SceneObjectRoot AcquireRoot(T category, string rootName)
        {
            this.EnsureContainer(category);

            return this.containers[category].AcquireRoot(rootName);
        }

        public SceneObjectRoot RegisterObjectAsRoot(T category, GameObject gameObject, bool persistent)
        {
            this.EnsureContainer(category);
            SceneObjectRoot root = this.containers[category].RegisterAsRoot(gameObject);
            root.Persistent = persistent;
            return root;
        }

        public void Clear(bool clearPersistentObjects = false)
        {
            foreach (SceneObjectContainer container in this.containers.Values)
            {
                container.Clear(clearPersistentObjects);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EnsureRootContainer()
        {
            if (this.rootContainer == null)
            {
                this.rootContainer = new SceneObjectContainer(null, RootName);

                // Attach the cleanup script
                var cleanup = this.rootContainer.GameObject.AddComponent<SceneObjectCleanup>();
                cleanup.OnCleanup += this.OnCleanup;
            }
        }
        
        private void EnsureContainer(T category)
        {
            this.EnsureRootContainer();

            if (!this.containers.ContainsKey(category))
            {
                this.containers[category] = new SceneObjectContainer(this.rootContainer, category.ToString(CultureInfo.InvariantCulture));
            }
        }

        private void OnCleanup()
        {
            foreach (SceneObjectContainer container in this.containers.Values)
            {
                container.Cleanup();
            }
        }
    }
}
