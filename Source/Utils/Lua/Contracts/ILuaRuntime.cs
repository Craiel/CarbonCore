namespace CarbonCore.Utils.Lua.Contracts
{
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Lua.Logic;

    public interface ILuaRuntime
    {
        void Register(ILuaObject luaObject);
        void Register(ILuaRuntimeFunction runtimeFunction);

        void Unregister(ILuaObject luaObject);
        void Unregister(ILuaRuntimeFunction runtimeFunction);

        void Reset(bool registerDefaults = true);

        LuaExecutionResult Execute(string script);
        LuaExecutionResult Execute(CarbonFile file);
    }
}
