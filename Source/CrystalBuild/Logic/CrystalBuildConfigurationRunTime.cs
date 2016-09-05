namespace CarbonCore.CrystalBuild.Logic
{
    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.Utils.Lua.Logic;

    using Utils.Lua.Logic.Library;

    public class CrystalBuildConfigurationRunTime : LuaRuntime, ICrystalBuildConfigurationRunTime
    {
        private static readonly LuaLibraryCrystalBuildConfig ConfigLibrary = new LuaLibraryCrystalBuildConfig();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CrystalBuildConfigurationRunTime()
        {
            LuaLibrary.RegisterLibrary(ConfigLibrary);

            // TODO: refactor and move to proper place
            LuaPreProcessor.DefineVariable("CB_VER", "1.0");
        }
    }
}
