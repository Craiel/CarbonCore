namespace CarbonCore.CrystalBuild.Sharp.Logic
{
    using System;
    using CarbonCore.Utils.Lua.Logic;
    using Utils.Lua.Logic.Library;

    public class CrystalBuildConfigurationRunTime : LuaRuntime, IDisposable
    {
        private readonly LuaLibraryCrystalBuildConfig library;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CrystalBuildConfigurationRunTime(CrystalBuildContext context)
        {
            this.library = new LuaLibraryCrystalBuildConfig(context);

            LuaLibrary.RegisterLibrary(this.library);

            // TODO: refactor and move to proper place
            LuaPreProcessor.DefineVariable("CB_VER", "1.0");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                LuaLibrary.UnregisterLibrary(this.library);
            }
        }
    }
}
