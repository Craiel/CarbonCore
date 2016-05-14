namespace CarbonCore.Utils.Lua.Logic.Core
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Lua.Contracts;

    public static class LuaCore
    {
        private static readonly LuaCoreLibrary CoreLibrary = new LuaCoreLibrary();

        public static readonly IDictionary<string, LuaCoreLibrary> Systems = new Dictionary<string, LuaCoreLibrary>();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static LuaCore()
        {
            Systems.Add(CoreLibrary.Name, CoreLibrary);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Register(ILuaRuntime target, string name)
        {
            LuaCoreLibrary library;
            if (!Systems.TryGetValue(name, out library))
            {
                throw new InvalidOperationException(string.Format("Library {0} does not exist in LuaCore", name));
            }

            library.Register(target);
        }

        public static void RegisterFullCore(ILuaRuntime target)
        {
            foreach (string name in Systems.Keys)
            {
                Systems[name].Register(target);
            }
        }
    }
}
