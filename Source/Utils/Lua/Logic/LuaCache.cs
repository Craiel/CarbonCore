namespace CarbonCore.Utils.Lua.Logic
{
    using System.Collections.Generic;
    
    public static class LuaCache
    {
        private static readonly IDictionary<LuaSource, LuaScript> Cache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static LuaCache()
        {
            Cache = new Dictionary<LuaSource, LuaScript>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Clear()
        {
            Cache.Clear();
        }

        public static LuaScript GetScript(LuaSource source)
        {
            LuaScript cachedResult;
            if (Cache.TryGetValue(source, out cachedResult))
            {
                if (!cachedResult.HasChanged)
                {
                    // Found a cached match, return without further processing
                    return cachedResult;
                }
            }

            return null;
        }

        public static LuaScript SetScript(LuaSource source, IList<string> data)
        {
            if (Cache.ContainsKey(source))
            {
                LuaScript existingEntry = Cache[source];
                existingEntry.Update(data);
                return existingEntry;
            }
            else
            {
                var entry = new LuaScript(source, data);
                Cache.Add(source, entry);
                return entry;
            }
        }
    }
}
