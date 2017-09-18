namespace CarbonCore.Applications.CrystalBuild.CSharp.Logic
{
    using System;
    using System.Collections.Generic;
    
    using CarbonCore.Applications.CrystalBuild.CSharp.Contracts;
    using CarbonCore.CrystalBuild.Sharp.Logic;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    using NLog;

    using Utils;
    using Utils.Lua.Logic;

    public class BuildLogic : IBuildLogic
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
            Logger.Info("Building {0} Sources", sources.Count);

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
                Logger.Warn("File to build not found: {0}", fileToBuild);
                return;
            }

            Logger.Info(" ------------------------ ");
            Logger.Info(" --> Building {0}", file);
            Logger.Info(string.Empty);

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
                    Logger.Error("Build of {0} failed", fileToBuild);
                }
            }

            Logger.Info(" --> Finished");
            Logger.Info(string.Empty);
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
