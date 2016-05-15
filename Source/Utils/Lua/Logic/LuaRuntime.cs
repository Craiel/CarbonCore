namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Lua.Contracts;
    using CarbonCore.Utils.Lua.Logic.Library;

    public class LuaRuntime : ILuaRuntime
    {
        private readonly IList<ILuaObject> objects;
        private readonly IList<ILuaRuntimeFunction> functions;
        private readonly IList<string> persistentScripts;

        private readonly HashSet<string> loadedLibraries;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaRuntime()
        {
            this.objects = new List<ILuaObject>();
            this.functions = new List<ILuaRuntimeFunction>();
            this.persistentScripts = new List<string>();

            this.loadedLibraries = new HashSet<string>();
            
            this.Reset();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Register(ILuaObject luaObject)
        {
            this.objects.Add(luaObject);
        }

        public void Register(ILuaRuntimeFunction runtimeFunction)
        {
            this.functions.Add(runtimeFunction);
        }

        public void Register(string persistentScript)
        {
            this.persistentScripts.Add(persistentScript);
        }
        
        public void Unregister(ILuaObject luaObject)
        {
            this.objects.Remove(luaObject);
        }

        public void Unregister(ILuaRuntimeFunction runtimeFunction)
        {
            this.functions.Remove(runtimeFunction);
        }

        public void Unregister(string persistentScript)
        {
            this.persistentScripts.Remove(persistentScript);
        }

        public LuaExecutionResult Execute(string script)
        {
            return this.DoExecute(script);
        }

        public LuaExecutionResult Execute(CarbonFile file)
        {
            return this.DoExecute(file);
        }

        public void Reset(bool registerDefaults = true)
        {
            this.objects.Clear();
            this.functions.Clear();

            if (registerDefaults)
            {
                DefaultFunctions.Instance.Register(this);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private NLua.Lua PrepareLua()
        {
            var lua = new NLua.Lua();

            foreach (ILuaObject luaObject in this.objects)
            {
                lua[luaObject.Name] = luaObject.Object;
            }

            foreach (ILuaRuntimeFunction function in this.functions)
            {
                function.Register(lua);
            }

            foreach (string script in this.persistentScripts)
            {
                lua.DoString(script);
            }

            return lua;
        }

        private LuaExecutionResult DoExecute(string script)
        {
            LuaExecutionResult result = new LuaExecutionResult();
            try
            {
                NLua.Lua lua = this.PrepareLua();
                result.ResultData = lua.DoString(script);
                result.Success = true;
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to execute Script: {0}", e);
                result.Exception = e;
            }

            return result;
        }

        private LuaExecutionResult DoExecute(CarbonFile file)
        {
            LuaScript script = LuaPreProcessor.Process(file);
            foreach (string libraryInclude in script.LibraryIncludes)
            {
                if (this.loadedLibraries.Contains(libraryInclude))
                {
                    continue;
                }

                if (!LuaLibrary.Register(this, libraryInclude))
                {
                    Diagnostic.Error("Aborting execution, required library {0} was not loaded", libraryInclude);
                }

                this.loadedLibraries.Add(libraryInclude);
            }

            return this.DoExecute(script.GetCompleteData());
        }
    }
}
