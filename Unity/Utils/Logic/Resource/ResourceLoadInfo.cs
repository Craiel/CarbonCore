namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using CarbonCore.Utils.Unity.Data;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public struct ResourceLoadInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadInfo(ResourceKey key, ResourceLoadFlags flags)
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
