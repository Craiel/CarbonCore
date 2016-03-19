namespace CarbonCore.Utils.Lua.Contracts
{
    using CarbonCore.Utils.IO;

    public interface ILuaRuntime
    {
        void Register(ILuaObject luaObject);
        void Register(ILuaRuntimeFunction runtimeFunction);

        void Unregister(ILuaObject luaObject);
        void Unregister(ILuaRuntimeFunction runtimeFunction);

        void Reset(bool registerDefaults = true);

        object[] Execute(string script);
        object[] Execute(CarbonFile file);
    }
}
