namespace CarbonCore.Utils.Lua.Contracts
{
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Lua.Logic;

    public interface ILuaRuntime
    {
        void Register(ILuaObject luaObject);
        void Register(ILuaRuntimeFunction runtimeFunction);
        void Register(string persistentScript);

        void Unregister(ILuaObject luaObject);
        void Unregister(ILuaRuntimeFunction runtimeFunction);
        void Unregister(string persistentScript);

        void Reset();

        LuaExecutionResult Execute(string script);
        LuaExecutionResult Execute(CarbonFile file);
    }
}
