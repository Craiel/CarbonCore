namespace CarbonCore.CrystalBuild.Logic
{
    using System.IO;

    using Utils.Diagnostics;
    using Utils.IO;
    using Utils.Lua.Contracts;
    using Utils.Lua.Logic;
    using Utils.Lua.Logic.Library;

    public class LuaLibraryCrystalBuildConfig : LuaLibraryBase
    {
        private readonly ILuaRuntimeFunction addSourcesFunction;
        private readonly ILuaRuntimeFunction addSourceFunction;
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public LuaLibraryCrystalBuildConfig()
            : base("CrystalBuildConfig")
        {
            this.addSourcesFunction = new LuaWrappedFunction("AddSources", this, "AddSources");
            this.addSourceFunction = new LuaWrappedFunction("AddSource", this, "AddSource");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void AddSources(string path, string pattern = "*", bool recursive = false)
        {
            Diagnostic.Warning("Adding sources from {0}", path);
            var directory = new CarbonDirectory(path);
            if (!directory.Exists)
            {
                Diagnostic.Warning("Skipping AddSources for {0}, does not exist", path);
                return;
            }

            CarbonFileResult[] matches = directory.GetFiles(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            Diagnostic.Info("Adding {0} files", matches.Length);
        }

        public void AddSource(string path)
        {
            Diagnostic.Info("Adding source {0}", path);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void RegisterCoreObjects(ILuaRuntime target)
        {
            base.RegisterCoreObjects(target);

            target.Register(this.addSourcesFunction);
            target.Register(this.addSourceFunction);
        }
    }
}
