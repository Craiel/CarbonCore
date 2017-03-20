namespace CarbonCore.Unity.Utils.Logic
{
    using CarbonCore.Unity.Utils.Logic.Json;
    using CarbonCore.Unity.Utils.Logic.Resource;
    using CarbonCore.Utils.Json;

    using Logging;

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

            UnityNLogRelay.InstantiateAndInitialize();
            ResourceProvider.InstantiateAndInitialize();
            BundleProvider.InstantiateAndInitialize();
            ResourceStreamProvider.InstantiateAndInitialize();
        }
    }
}
