namespace CarbonCore.Utils.Unity.Logic
{
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Json;
    using CarbonCore.Utils.Unity.Logic.Json;
    using CarbonCore.Utils.Unity.Logic.Resource;

    using UnityEngine;

    public class CarbonEngine : UnitySingletonBehavior<CarbonEngine>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            UnityDebugTraceListener.Setup();

            JsonExtensions.RegisterGlobalConverter<Vector3, Vector3ConverterSmall>();
            JsonExtensions.RegisterGlobalConverter<Quaternion, QuaternionConverterSmall>();
            JsonExtensions.RegisterGlobalConverter<Color, ColorConverter>();

            // Initialize a default log instance
            this.InitializeLog<DefaultEngineLog>();

            ResourceProvider.InstantiateAndInitialize();
            BundleProvider.InstantiateAndInitialize();
            ResourceStreamProvider.InstantiateAndInitialize();
        }

        public void InitializeLog<T>()
            where T : ILog
        {
            Diagnostic.SetInstance(new CarbonDiagnostics<T, MetricProvider>());

            Diagnostic.UnregisterThread();
            Diagnostic.RegisterThread(this.GetType().Name);
        }
    }
}
