namespace CarbonCore.Utils.Lua.Logic.Library
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Lua.Contracts;

    public class LuaLibrarySystem : LuaLibraryBase
    {
        private readonly ILuaRuntimeFunction printLineFunction;

        private readonly ILuaRuntimeFunction infoFunction;
        private readonly ILuaRuntimeFunction warningFunction;
        private readonly ILuaRuntimeFunction errorFunction;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public LuaLibrarySystem()
            : base("System")
        {
            this.printLineFunction = new LuaWrappedFunction("PrintLine", this, "LogInfo");

            this.infoFunction = new LuaWrappedFunction("LogInfo", this, "LogInfo");
            this.warningFunction = new LuaWrappedFunction("LogWarning", this, "LogWarning");
            this.errorFunction = new LuaWrappedFunction("LogError", this, "LogError");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void LogInfo(string message)
        {
            Diagnostic.Info(message);
        }

        public void LogWarning(string message)
        {
            Diagnostic.Warning(message);
        }

        public void LogError(string message)
        {
            Diagnostic.Error(message);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void RegisterCoreObjects(ILuaRuntime target)
        {
            base.RegisterCoreObjects(target);

            target.Register(this.printLineFunction);
            target.Register(this.infoFunction);
            target.Register(this.warningFunction);
            target.Register(this.errorFunction);
        }
    }
}
