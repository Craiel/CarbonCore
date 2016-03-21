namespace CarbonCore.Utils.Lua.Contracts
{
    using NLua;

    public interface ILuaRuntimeFunction
    {
        string Name { get; }
        
        void Register(Lua target);
    }
}
