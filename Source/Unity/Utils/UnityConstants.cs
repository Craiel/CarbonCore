namespace CarbonCore.Unity.Utils
{
    public static class UnityConstants
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public const string UnityPathSeparator = "/";

        public const string UnityExtensionMeta = ".meta";
        public const string UnityExtensionManifest = ".manifest";
        public const string UnityExtensionPrefab = ".prefab";
        public const string UnityExtensionMaterial = ".mat";
        public const string UnityExtensionOverrideController = ".overrideController";
        public const string UnityExtensionController = ".controller";

        public const string SceneControllerRoot = "SceneRoot";

#if DEBUG
        public static bool EnableVerboseLogging = true;
#else
        public static bool EnableVerboseLogging = false;
#endif
    }
}
