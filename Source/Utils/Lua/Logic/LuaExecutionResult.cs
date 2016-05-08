namespace CarbonCore.Utils.Lua.Logic
{
    using System;

    public class LuaExecutionResult
    {
        public bool Success { get; set; }

        public object[] ResultData { get; set; }

        public Exception Exception { get; set; }
    }
}
