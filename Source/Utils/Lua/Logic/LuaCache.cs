namespace CarbonCore.Utils.Lua.Logic
{
    using System.Collections.Generic;
    
    public static class LuaCache
    {
        private static readonly IDictionary<LuaSource, LuaCachedScript> Cache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static LuaCache()
        {
            Cache = new Dictionary<LuaSource, LuaCachedScript>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Clear()
        {
            Cache.Clear();
        }

        public static LuaCachedScript GetScript(LuaSource source)
        {
            LuaCachedScript cachedResult;
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

        public static LuaCachedScript SetScript(LuaSource source, IList<string> data)
        {
            if (Cache.ContainsKey(source))
            {
                LuaCachedScript existingEntry = Cache[source];
                existingEntry.Update(data);
                return existingEntry;
            }
            else
            {
                var entry = new LuaCachedScript(source, data);
                Cache.Add(source, entry);
                return entry;
            }
        }
    }
}
