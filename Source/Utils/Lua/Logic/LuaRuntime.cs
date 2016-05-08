namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Lua.Contracts;

    public class LuaRuntime : ILuaRuntime
    {
        private readonly IList<ILuaObject> objects;
        private readonly IList<ILuaRuntimeFunction> functions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaRuntime()
        {
            this.objects = new List<ILuaObject>();
            this.functions = new List<ILuaRuntimeFunction>();

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

        public void Unregister(ILuaObject luaObject)
        {
            this.objects.Remove(luaObject);
        }

        public void Unregister(ILuaRuntimeFunction runtimeFunction)
        {
            this.functions.Remove(runtimeFunction);
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
            LuaExecutionResult result = new LuaExecutionResult();

            try
            {
                NLua.Lua lua = this.PrepareLua();
                result.ResultData = lua.DoFile(file.GetPath());
                result.Success = true;
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to execute Script: {0}", e);
                result.Exception = e;
            }

            return result;
        }
    }
}
