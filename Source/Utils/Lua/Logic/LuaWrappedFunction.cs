namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Reflection;

    using CarbonCore.Utils.Lua.Contracts;

    using NLua;

    public class LuaWrappedFunction : ILuaRuntimeFunction
    {
        private readonly object owner;
        private readonly MethodBase method;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaWrappedFunction(string name, object owner, string functionName)
        {
            this.Name = name;
            this.owner = owner;
            this.method = owner.GetType().GetMethod(functionName);

            if (this.method == null)
            {
                throw new InvalidOperationException("LuaWrappedFunction requires valid method");
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public void Register(Lua target)
        {
            target.RegisterFunction(this.Name, this.owner, this.method);
        }
    }
}
