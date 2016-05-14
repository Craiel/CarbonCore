namespace CarbonCore.Utils.Lua.Logic.Core
{
    using System;

    using CarbonCore.Utils.Lua.Contracts;

    public class LuaCoreBase
    {
        private static readonly LuaPreProcessor CorePreProcessor = new LuaPreProcessor();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaCoreBase(string name)
        {
            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public bool IsInitialized { get; private set; }
        
        public string ScriptData { get; private set; }

        public void Initialize()
        {
            Type type = this.GetType();

            string rawScriptData = type.Assembly.LoadResourceAsString(string.Concat(this.Name, Constants.ExtensionLua));

            // No need to cache core scripts, they are already cached in here
            this.ScriptData = CorePreProcessor.Process(rawScriptData, false);
            Diagnostics.Diagnostic.Info("Loaded Lua Core Script {0} with {1} characters", this.Name, this.ScriptData.Length);
        }

        public virtual void Register(ILuaRuntime target)
        {
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
            LuaExecutionResult result = target.Execute(this.ScriptData);
            if (result == null)
            {
                Diagnostics.Diagnostic.Error("Failed to Register core lua {0}, no execution result returned!", this.Name);
                return;
            }

            if (!result.Success)
            {
                Diagnostics.Diagnostic.Error("Failed to Register core lua {0}: {1}", this.Name, result.Exception);
            }
        }
    }
}
