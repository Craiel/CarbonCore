namespace CarbonCore.Unity.Utils.Logic
{
    using System;

    using CarbonCore.Unity.Utils.Contracts;
    using CarbonCore.Unity.Utils.Logic.Scene;
    using CarbonCore.Utils.Unity.Logic.Scene;

    using NLog;

    using UnityEngine;
    
    public abstract class UnitySingletonBehavior<T> : MonoBehaviour, IUnitySingletonBehavior
        where T : UnitySingletonBehavior<T>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private static T instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool AutoInstantiate;

        public static bool IsInstanceActive => instance != default(T);

        public static T Instance => instance;

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
                    Logger.Error("Error trying to add Singleton Component {0}: {1}", typeof(T), e);
                }

                if (instance == null)
                {
                    Logger.Error("Adding Component of type {0} returned null", typeof(T));
                }

                // Only attempt Don't destroy if the object has no parent
                if (gameObject.transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
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
            if (instance == null && this.AutoInstantiate)
            {
                instance = (T)this;
            }

            if (instance != null && instance != this)
            {
                Logger.Error("Duplicate Instance of {0} found, destroying!", this.GetType());
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
            Logger.Info("Destroying Singleton MonoBehavior {0}", this.name);

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
