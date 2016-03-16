namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using CarbonCore.Unity.Utils.Data;
    using CarbonCore.Unity.Utils.Logic.Enums;

    public struct BundleLoadInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BundleLoadInfo(BundleKey key, BundleLoadFlags flags)
            : this()
        {
            this.Key = key;
            this.Flags = flags;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BundleKey Key { get; private set; }

        public BundleLoadFlags Flags { get; private set; }
    }
}
