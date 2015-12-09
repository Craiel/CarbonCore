﻿namespace CarbonCore.Utils.Unity.Logic
{
    using System;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Logic.Scene;

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

            parent.RegisterObjectAsRoot(category, this.GameObject, true);
        }

        protected void RegisterAsChild<TN>(SceneObjectController<TN> controller, TN category, string parent)
            where TN : struct, IConvertible
        {
            Diagnostic.Assert(this.GameObject != null);

            SceneObjectRoot root = controller.AcquireRoot(category, parent);
            root.AddChild(this.GameObject);
        }

        protected void DisposeSingleton()
        {
            Diagnostic.Info("Disposing Singleton {0}", this.Name);

            Instance = null;
        }
    }
}