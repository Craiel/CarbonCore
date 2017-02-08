namespace CarbonCore.Utils.Lua.Logic.Library
{
    using System;
    using System.Collections.Generic;
    using CarbonCore.Utils.Lua.Contracts;

    public class LuaLibraryBase
    {
        private const string LibraryPrefix = @"CarbonCore.Utils.Lua.Resources.LuaLib";

        private readonly IList<ILuaRuntimeFunction> managedLocalFunctions;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaLibraryBase(string name)
        {
            this.Name = name;

            this.managedLocalFunctions = new List<ILuaRuntimeFunction>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool IsInitialized { get; private set; }
        
        public LuaScript Script { get; private set; }

        public void Initialize()
        {
            this.LoadResourceScript();
        }

        public virtual void Register(ILuaRuntime target)
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
            }

            this.RegisterCoreObjects(target);
            this.RegisterCoreLua(target);
        }

        public virtual void Unregister(ILuaRuntime target)
        {
            if (!this.IsInitialized)
            {
                throw new InvalidOperationException("Tried to unregister a non-initialized Lua Library");
            }

            this.UnregisterCoreObjects(target);
            this.UnregisterCoreLua(target);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void LoadResourceScript()
        {
            Type type = this.GetType();

            string rawScriptData = type.Assembly.LoadResourceAsString(string.Concat(LibraryPrefix, this.Name, Constants.ExtensionLua));
            if (rawScriptData == null)
            {
                Diagnostics.Diagnostic.Warning("Lua Library {0} has no Script Data!", this.Name);
                return;
            }

            // No need to cache core scripts, they are already cached in here
            this.Script = LuaPreProcessor.Process(rawScriptData, false);
            Diagnostics.Diagnostic.Info("Loaded Lua Core Script {0} with {1} characters", this.Name, this.Script.Data.Count);
        }

        protected virtual void RegisterCoreObjects(ILuaRuntime target)
        {
            // This is where the class registers it's objects as needed by the script
            foreach (ILuaRuntimeFunction function in this.managedLocalFunctions)
            {
                target.Register(function);
            }
        }

        protected virtual void UnregisterCoreObjects(ILuaRuntime target)
        {
            foreach (ILuaRuntimeFunction function in this.managedLocalFunctions)
            {
                target.Unregister(function);
            }
        }

        protected virtual void RegisterCoreLua(ILuaRuntime target)
        {
            if (this.Script == null)
            {
                return;
            }

            target.Register(this.Script.GetCompleteData());
        }

        protected virtual void UnregisterCoreLua(ILuaRuntime target)
        {
            if (this.Script == null)
            {
                return;
            }

            target.Unregister(this.Script.GetCompleteData());
        }

        protected void AddLibraryFunction<T, T1, T2>(Action<T, T1, T2> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction<T, T1>(Action<T, T1> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction<T>(Action<T> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction(Action action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction<T1, T2, TR>(Func<T1, T2, TR> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction<T1, TR>(Func<T1, TR> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }
        
        protected void AddLibraryFunction<TR>(Func<TR> action)
        {
            this.AddLibraryFunction(action.Method.Name);
        }

        protected void AddLibraryFunction(string name)
        {
            var function = new LuaWrappedFunction(name, this, name);
            this.managedLocalFunctions.Add(function);
        }
    }
}
