namespace CarbonCore.Utils.Unity.Logic.Enums
{
    using System;

    [Flags]
    public enum ResourceLoadFlags
    {
        None = 0,
        Instantiate = 1 << 0,
        Sync = 1 << 1,
        Cache = 1 << 2,
    }
}
