namespace CarbonCore.CrystalBuild.Logic
{
    using System;
    using System.IO;

    using Config;

    using NLog;

    using Utils.IO;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public static class BuildLogic
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool Build(BuildContext context)
        {
            Logger.Info("Building with {0} config files", context.ConfigFiles.Count);
            
            if (!ReadConfigs(context))
            {
                return false;
            }

            if (!AnalyzeConfigs(context))
            {
                return false;
            }

            // TODO:
            //  - Resolve inheritance
            //  - Build project file

            return true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static bool ReadConfigs(BuildContext context)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            foreach (CarbonFile file in context.ConfigFiles)
            {
                try
                {
                    CarbonFile existingFile = file;
                    YamlCrystalConfig config = ReadConfig(context, ref existingFile, deserializer);
                    if (config == null)
                    {
                        Logger.Error($"Failed to read config for {file}");
                        return false;
                    }

                    Logger.Info($" -> {file}");
                    context.ConfigCache.Add(file, config);
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Failed to read config for {file}");
                    return false;
                }
            }

            return true;
        }

        private static YamlCrystalConfig ReadConfig(BuildContext context, ref CarbonFile file, Deserializer deserializer)
        {
            if (!file.Exists)
            {
                file = context.Root.ToFile(file);
                if (!file.Exists)
                {
                    Logger.Error($"Config File {file} not found");
                }
            }
            
            using (var stream = file.OpenRead())
            {
                using (var reader = new StreamReader(stream))
                {
                    return deserializer.Deserialize<YamlCrystalConfig>(reader);
                }
            }
        }

        private static bool AnalyzeConfigs(BuildContext context)
        {
            foreach (CarbonFile file in context.ConfigCache.Keys)
            {
                YamlCrystalConfig config = context.ConfigCache[file];
                foreach (string key in config.Variables.Keys)
                {
                    if (context.Variables.ContainsKey(key))
                    {
                        Logger.Error($"Duplicate variable defined: {key} in {file}");
                        return false;
                    }

                    context.Variables.Add(key, context.Variables[key]);
                }

                foreach (YamlBuildConfig buildConfig in config.BuildConfigs)
                {
                    if (context.BuildConfigs.ContainsKey(buildConfig.Id))
                    {
                        Logger.Error($"Duplicate build config defined: {buildConfig.Id} in {file}");
                        return false;
                    }

                    context.BuildConfigs.Add(buildConfig.Id, buildConfig);
                }

                foreach (YamlProject project in config.Projects)
                {
                    if (context.Projects.ContainsKey(project.Id))
                    {
                        Logger.Error($"Duplicate project defined: {project.Id} in {file}");
                        return false;
                    }

                    context.Projects.Add(project.Id, project);
                }

                foreach (YamlReference reference in config.References)
                {
                    if (context.References.ContainsKey(reference.Id))
                    {
                        Logger.Error($"Duplicate reference defined: {reference.Id} in {file}");
                        return false;
                    }

                    context.References.Add(reference.Id, reference);
                }
            }

            Logger.Info($" = {context.Variables.Count} Variables");
            Logger.Info($" = {context.Projects.Count} Projects");
            Logger.Info($" = {context.BuildConfigs.Count} Build Configurations");
            Logger.Info($" = {context.References.Count} References");

            return true;
        }

        /*private void BuildSource(CarbonFile file, CarbonDirectory projectRoot)
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
                    throw new InvalidOperationException("Build failed!");
                }
            }

            Logger.Info(" --> Finished");
            Logger.Info(string.Empty);
        }*/

        /*private bool PrepareConfigurationRuntime(CrystalBuildContext context, CarbonFile file, CarbonDirectory projectRoot)
        {
            context.ProjectRoot = projectRoot;
            context.BuildDir = file.ToAbsolute<CarbonFile>(context.ProjectRoot).GetDirectory();

            LuaPreProcessor.DefineVariableFromPath("PROJECT_ROOT", context.ProjectRoot);
            LuaPreProcessor.DefineVariableFromPath("BUILD_DIR", context.BuildDir);

            return true;
        }*/
    }
}
