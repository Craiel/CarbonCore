namespace CarbonCore.Utils.Lua.IoC
{
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Lua.Contracts;
    using CarbonCore.Utils.Lua.Logic;

    [DependsOnModule(typeof(UtilsModule))]
    public class UtilsLuaModule : CarbonQuickModule
    {
        public UtilsLuaModule()
        {
            this.For<ILuaRuntime>().Use<LuaRuntime>();
        }
    }
}