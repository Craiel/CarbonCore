namespace CarbonCore.Utils.Lua.Logic.Library
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Lua.Contracts;

    public class LuaLibrarySystem : LuaLibraryBase
    {
        public readonly ILuaRuntimeFunction PrintLineFunction;

        public readonly ILuaRuntimeFunction InfoFunction;
        public readonly ILuaRuntimeFunction WarningFunction;
        public readonly ILuaRuntimeFunction ErrorFunction;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public LuaLibrarySystem()
            : base("System")
        {
            this.PrintLineFunction = new LuaWrappedFunction("PrintLine", this, "LogInfo");

            this.InfoFunction = new LuaWrappedFunction("LogInfo", this, "LogInfo");
            this.WarningFunction = new LuaWrappedFunction("LogWarning", this, "LogWarning");
            this.ErrorFunction = new LuaWrappedFunction("LogError", this, "LogError");
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

            target.Register(this.PrintLineFunction);
            target.Register(this.InfoFunction);
            target.Register(this.WarningFunction);
            target.Register(this.ErrorFunction);
        }
    }
}
