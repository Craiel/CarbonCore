namespace CarbonCore.Utils.Lua.Logic.Library
{
    using System;

    using CarbonCore.Utils.Lua.Contracts;

    public class LuaLibraryBase
    {
        private const string LibraryPrefix = @"CarbonCore.Utils.Lua.Resources.LuaLib";

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaLibraryBase(string name)
        {
            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool IsInitialized { get; private set; }
        
        public LuaScript Script { get; private set; }

        public void Initialize()
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

        public virtual void Register(ILuaRuntime target)
        {
            if (!this.IsInitialized)
            {
                this.Initialize();
            }

            this.RegisterCoreObjects(target);
            this.RegisterCoreLua(target);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void RegisterCoreObjects(ILuaRuntime target)
        {
            // This is where the class registers it's objects as needed by the script
        }

        protected virtual void RegisterCoreLua(ILuaRuntime target)
        {
            if (this.Script == null)
            {
                return;
            }

            target.Register(this.Script.GetCompleteData());
        }
    }
}
