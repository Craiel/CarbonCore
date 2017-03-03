namespace CarbonCore.Applications.CrystalBuild.CSharp.Logic
{
    using System;
    using System.Collections.Generic;
    
    using CarbonCore.Applications.CrystalBuild.CSharp.Contracts;
    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.CrystalBuild.Sharp.Logic;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;
    using Utils;
    using Utils.Lua.Logic;

    public class BuildLogic : IBuildLogic
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildLogic(IFactory factory)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void BuildProjectFile(IList<CarbonFile> sources, CarbonDirectory projectRoot)
        {
            Diagnostic.Info("Building {0} Sources", sources.Count);

            foreach (CarbonFile source in sources)
            {
                this.BuildSource(source, projectRoot);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void BuildSource(CarbonFile file, CarbonDirectory projectRoot)
        {
            CarbonFile fileToBuild = file.Exists ? file : projectRoot.ToFile(file);
            if (!fileToBuild.Exists)
            {
                Diagnostic.Warning("File to build not found: {0}", fileToBuild);
                return;
            }

            Diagnostic.Info(" ------------------------ ");
            Diagnostic.Info(" --> Building {0}", file);
            Diagnostic.Info(string.Empty);

            using (var region = new ProfileRegion("CrystalBuild.Execute"))
            {
                var context = new CrystalBuildContext();
                using (var configRuntime = new CrystalBuildConfigurationRunTime(context))
                {

                    if (!this.PrepareConfigurationRuntime(context, fileToBuild, projectRoot))
                    {
                        return;
                    }

                    LuaExecutionResult result = configRuntime.Execute(fileToBuild);
                    if (!result.Success)
                    {
                        throw new InvalidOperationException("Build failed!");
                    }
                }
                
                Diagnostic.Info(" --> Finished in {0}", Timer.TimeToTimeSpan(region.ElapsedTicks));
                Diagnostic.Info(string.Empty);
            }
        }

        private bool PrepareConfigurationRuntime(CrystalBuildContext context, CarbonFile file, CarbonDirectory projectRoot)
        {
            context.ProjectRoot = projectRoot;
            context.BuildDir = file.ToAbsolute<CarbonFile>(context.ProjectRoot).GetDirectory();

            LuaPreProcessor.DefineVariableFromPath("PROJECT_ROOT", context.ProjectRoot);
            LuaPreProcessor.DefineVariableFromPath("BUILD_DIR", context.BuildDir);

            return true;
        }
    }
}
