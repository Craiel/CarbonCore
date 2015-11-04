namespace CarbonCore.Utils.Unity.Scene
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using UnityEngine;

    public class SceneObjectController<T> : UnitySingleton<SceneObjectController<T>>
        where T : struct, IConvertible
    {
        private const string RootName = "Scene";

        private readonly IDictionary<T, SceneContainer> containers;

        private SceneContainer rootContainer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectController()
            : base(RootName)
        {
            this.containers = new Dictionary<T, SceneContainer>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SceneObjectRoot AcquireRoot(T category, string rootName)
        {
            this.EnsureContainer(category);

            return this.containers[category].AcquireRoot(rootName);
        }

        public SceneObjectRoot RegisterObjectAsRoot(T category, GameObject gameObject)
        {
            this.EnsureContainer(category);

            return this.containers[category].RegisterAsRoot(gameObject);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EnsureRootContainer()
        {
            if (this.rootContainer == null)
            {
                this.rootContainer = new SceneContainer(null, this.Name);
            }
        }

        private void EnsureContainer(T category)
        {
            this.EnsureRootContainer();

            if (!this.containers.ContainsKey(category))
            {
                this.containers[category] = new SceneContainer(this.rootContainer, category.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
