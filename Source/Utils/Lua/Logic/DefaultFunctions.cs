namespace CarbonCore.Utils.Lua.Logic
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Lua.Contracts;
    
    public class DefaultFunctions
    {
        public static readonly DefaultFunctions Instance = new DefaultFunctions();

        public static readonly ILuaRuntimeFunction EnumRuntimeFunction = new LuaNativeFunction("function enum(o)\nlocal e = o:GetEnumerator()\nreturn function()\nif e:MoveNext() then return e.Current end end end");
        public static readonly ILuaRuntimeFunction PrintToPrintLineFunction = new LuaNativeFunction("function print(...)\nlocal line = \"\"\nfor k, v in pairs({...}) do\nline = line .. tostring(v)\nend\nPrintLine(line)\nend");

        public readonly ILuaRuntimeFunction PrintLineFunction;

        public readonly ILuaRuntimeFunction InfoFunction;
        public readonly ILuaRuntimeFunction WarningFunction;
        public readonly ILuaRuntimeFunction ErrorFunction;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DefaultFunctions()
        {
            this.PrintLineFunction = new LuaWrappedFunction("PrintLine", this, "LogInfo");

            this.InfoFunction = new LuaWrappedFunction("LogInfo", this, "LogInfo");
            this.WarningFunction = new LuaWrappedFunction("LogWarning", this, "LogWarning");
            this.ErrorFunction = new LuaWrappedFunction("LogError", this, "LogError");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Register(ILuaRuntime runtime)
        {
            runtime.Register(EnumRuntimeFunction);
            runtime.Register(PrintToPrintLineFunction);

            runtime.Register(this.PrintLineFunction);

            runtime.Register(this.InfoFunction);
            runtime.Register(this.WarningFunction);
            runtime.Register(this.ErrorFunction);
        }

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
    }
}
