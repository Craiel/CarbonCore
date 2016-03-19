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

        public object[] Execute(string script)
        {
            return this.DoExecute(script);
        }

        public object[] Execute(CarbonFile file)
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

        private object[] DoExecute(string script)
        {
            try
            {
                NLua.Lua lua = this.PrepareLua();
                return lua.DoString(script);
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to execute Script: {0}", e);
                throw;
            }
        }

        private object[] DoExecute(CarbonFile file)
        {
            try
            {
                NLua.Lua lua = this.PrepareLua();
                return lua.DoFile(file.GetPath());
            }
            catch (Exception e)
            {
                Diagnostic.Error("Failed to execute Script: {0}", e);
                throw;
            }
        }
    }
}
