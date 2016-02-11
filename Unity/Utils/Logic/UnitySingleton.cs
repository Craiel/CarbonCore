namespace CarbonCore.Utils.Unity.Logic
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Contracts;

    public abstract class UnitySingleton<T> : IUnitySingleton
        where T : class, IUnitySingleton, new()
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInstanceActive
        {
            get
            {
                return Instance != null;
            }
        }

        public static T Instance { get; private set; }

        public bool IsInitialized { get; protected set; }

        public static void Instantiate()
        {
            // Prevent accidential duplicate initialization
            if (Instance != null)
            {
                return;
            }

            Instance = new T();
        }
        
        public static void InstantiateAndInitialize()
        {
            if (Instance != null && Instance.IsInitialized)
            {
                // Instance is already there and initialized, skip
                return;
            }

            Instantiate();
            Instance.Initialize();
        }

        public virtual void Initialize()
        {
            Diagnostic.Info("Initializing Singleton {0}", this.GetType());

            this.IsInitialized = true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void DestroySingleton()
        {
            Diagnostic.Info("Destroying Singleton {0}", this.GetType());

            Instance = null;
        }
    }
}
