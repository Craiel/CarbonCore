namespace CarbonCore.Unity.Utils.Logic
{
    using CarbonCore.Unity.Utils.Contracts;

    using NLog;

    public abstract class UnitySingleton<T> : IUnitySingleton
        where T : class, IUnitySingleton, new()
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            Logger.Info("Initializing Singleton {0}", this.GetType());

            this.IsInitialized = true;
        }

        public static void DestroyInstance()
        {
            if (IsInstanceActive)
            {
                Instance.DestroySingleton();
            }
        }

        public virtual void DestroySingleton()
        {
            Logger.Info("Destroying Singleton {0}", this.GetType());

            Instance = null;
        }
    }
}
