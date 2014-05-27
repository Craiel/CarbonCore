namespace CarbonCore.JSharpBridge
{
    using System.Collections;
    using System.Collections.Generic;

    public static class BridgeCollections
    {
        public static void AddAll(ISet<object> target, IEnumerable source)
        {
            foreach (object entry in source)
            {
                target.Add(entry);
            }
        }
    }
}
