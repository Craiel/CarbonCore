namespace CarbonCore.Utils.Unity
{
    using System;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Unity.Scene;

    using UnityEngine;

    public abstract class UnitySingleton<T>
        where T : class, new()
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static UnitySingleton()
        {
            Instance = new T();
        }

        protected UnitySingleton(string name = null)
        {
            this.Name = name ?? typeof(T).Name;

            Diagnostic.Info("UnitySingleton Initialized: {0}", this.Name);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static T Instance { get; private set; }

        public GameObject GameObject { get; private set; }

        public string Name { get; private set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void CreateGameObject()
        {
            this.GameObject = new GameObject(this.Name);
        }

        protected void CreateRootObject<TN>(SceneObjectController<TN> parent, TN category)
            where TN : struct, IConvertible
        {
            this.CreateGameObject();

            parent.RegisterObjectAsRoot(category, this.GameObject);
        }

        protected void RegisterAsChild<TN>(SceneObjectController<TN> controller, TN category, string parent)
            where TN : struct, IConvertible
        {
            Diagnostic.Assert(this.GameObject != null);

            SceneObjectRoot root = controller.AcquireRoot(category, parent);
            root.AddChild(this.GameObject);
        }
    }
}
