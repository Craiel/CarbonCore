namespace CarbonCore.Unity.Utils.Logic
{
    using CarbonCore.Unity.Utils.Logic.Json;
    using CarbonCore.Unity.Utils.Logic.Resource;
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Json;

    using UnityEngine;

    public class CarbonEngine : UnitySingletonBehavior<CarbonEngine>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            JsonExtensions.RegisterGlobalConverter<Vector3, Vector3ConverterSmall>();
            JsonExtensions.RegisterGlobalConverter<Quaternion, QuaternionConverterSmall>();
            JsonExtensions.RegisterGlobalConverter<Color, ColorConverter>();

            // Initialize a default log instance
            this.InitializeLog<DefaultEngineLog>();

            ResourceProvider.InstantiateAndInitialize();
            BundleProvider.InstantiateAndInitialize();
            ResourceStreamProvider.InstantiateAndInitialize();
        }

        public void InitializeLog<T>(bool useDefaultTraceListener = false)
            where T : ILog
        {
            if (useDefaultTraceListener)
            {
                UnityDebugTraceListener.Setup();
            }

            Diagnostic.SetInstance(new CarbonDiagnostics<T, MetricProvider>());

            Diagnostic.UnregisterThread();
            Diagnostic.RegisterThread(this.GetType().Name);
        }
    }
}
