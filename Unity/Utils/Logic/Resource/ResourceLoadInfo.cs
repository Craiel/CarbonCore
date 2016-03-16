namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using CarbonCore.Unity.Utils.Data;
    using CarbonCore.Unity.Utils.Logic.Enums;

    public struct ResourceLoadInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadInfo(ResourceKey key, ResourceLoadFlags flags)
            : this()
        {
            this.Key = key;
            this.Flags = flags;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceKey Key { get; private set; }

        public ResourceLoadFlags Flags { get; private set; }
    }
}
