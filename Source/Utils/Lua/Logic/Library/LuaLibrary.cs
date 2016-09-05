﻿namespace CarbonCore.Utils.Lua.Logic.Library
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Lua.Contracts;

    public static class LuaLibrary
    {
        private static readonly LuaLibrarySystem SystemLibrary = new LuaLibrarySystem();

        private static readonly IDictionary<string, LuaLibraryBase> LibraryRegistry = new Dictionary<string, LuaLibraryBase>();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static LuaLibrary()
        {
            RegisterLibrary(SystemLibrary);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void RegisterLibrary(LuaLibraryBase library)
        {
            LibraryRegistry.Add(library.Name, library);
        }

        public static bool Register(ILuaRuntime target, string name)
        {
            LuaLibraryBase library;
            if (!LibraryRegistry.TryGetValue(name, out library))
            {
                Diagnostics.Diagnostic.Error("Library {0} does not exist in LuaLibrary", name);
                return false;
            }

            library.Register(target);
            return true;
        }

        public static void RegisterAll(ILuaRuntime target)
        {
            foreach (string name in LibraryRegistry.Keys)
            {
                LibraryRegistry[name].Register(target);
            }
        }
    }
}
