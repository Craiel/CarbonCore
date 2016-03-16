namespace CarbonCore.Unity.Utils.Logic
{
    using System;

    using CarbonCore.Unity.Utils.Contracts;
    using CarbonCore.Unity.Utils.Logic.Scene;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Logic.Scene;

    using UnityEngine;

    public abstract class UnitySingletonBehavior<T> : MonoBehaviour, IUnitySingletonBehavior
        where T : UnitySingletonBehavior<T>
    {
        private static T instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInstanceActive
        {
            get
            {
                return instance != default(T);
            }
        }

        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsInitialized { get; protected set; }

        public static void Instantiate()
        {
            if (Instance != null)
            {
                return;
            }

            instance = FindObjectOfType<T>();
            
            if (instance == null)
            {
                GameObject gameObject = new GameObject(typeof(T).Name);

                try
                {
                    instance = gameObject.AddComponent<T>();
                }
                catch (Exception e)
                {
                    Diagnostic.Error("Error trying to add Singleton Component {0}: {1}", typeof(T), e);
                }

                if (instance == null)
                {
                    Diagnostic.Error("Adding Component of type {0} returned null", typeof(T));
                }

                DontDestroyOnLoad(gameObject);
            }
        }

        public static void InstantiateAndInitialize()
        {
            if (Instance != null && Instance.IsInitialized)
            {
                return;
            }

            Instantiate();
            Instance.Initialize();
        }

        public static void Destroy()
        {
            instance.DestroySingleton();
        }

        public virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Diagnostic.Error("Duplicate Instance of {0} found, destroying!", this.GetType());
                DestroyImmediate(this.gameObject);
            }
        }

        public virtual void Initialize()
        {
            this.IsInitialized = true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected SceneObjectRoot RegisterInController<TN>(SceneObjectController<TN> parent, TN category, bool persistent = false)
            where TN : struct, IConvertible
        {
            SceneObjectRoot root = parent.RegisterObjectAsRoot(category, this.gameObject, persistent);

            root.OnDestroying += this.OnSingletonDestroying;

            return root;
        }
        
        protected void DestroySingleton()
        {
            Diagnostic.Info("Destroying Singleton MonoBehavior {0}", this.name);

            this.OnSingletonDestroying();

            Destroy(this.gameObject);
        }

        protected virtual void OnSingletonDestroying()
        {
            // Clear out the instance since our parent got destroyed
            instance = null;
        }
    }
}
