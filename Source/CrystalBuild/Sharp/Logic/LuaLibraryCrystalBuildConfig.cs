﻿namespace CarbonCore.CrystalBuild.Sharp.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Config;

    using Data;
    using Data.CSP;
    using Enums;

    using NLog;

    using Utils;
    using Utils.IO;
    using Utils.Lua.Logic.Library;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    public class LuaLibraryCrystalBuildConfig : LuaLibraryBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CrystalBuildContext context;
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public LuaLibraryCrystalBuildConfig(CrystalBuildContext context)
            : base("CrystalBuildConfig")
        {
            this.context = context;
            
            this.AddLibraryFunction<string, string, bool>(this.AddSources);
            this.AddLibraryFunction<string>(this.AddSource);
            this.AddLibraryFunction<string>(this.AddConfig);
            this.AddLibraryFunction<string, string, bool>(this.AddContent);

            this.AddLibraryFunction<string>(this.SetEntryPoint);
            this.AddLibraryFunction<string>(this.SetDefaultOutputPath);
            this.AddLibraryFunction<string>(this.SetDefaultIntermediateOutputPath);
            this.AddLibraryFunction<string>(this.SetCodeAnalysisRules);
            this.AddLibraryFunction<string>(this.SetFramework);
            this.AddLibraryFunction<string>(this.SetFrameworkProfile);
            this.AddLibraryFunction<string>(this.SetOutputType);
            this.AddLibraryFunction<string>(this.SetNamespace);
            this.AddLibraryFunction<string>(this.SetIcon);

            this.AddLibraryFunction(this.AddStandardReferences);
            this.AddLibraryFunction<string, string>(this.AddReference);
            this.AddLibraryFunction<string, string, bool>(this.AddProjectReference);

            this.AddLibraryFunction<string, string, BuildConfigObject>(this.CreateBuildConfig);

            this.AddLibraryFunction(this.WriteProjectFile);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void AddSources(string path, string pattern = "*.cs", bool recursive = false)
        {
            Logger.Info("Adding sources from {0}", path);
            var directory = new CarbonDirectory(path);
            if (!directory.Exists)
            {
                Logger.Warn("Skipping AddSources for {0}, does not exist", path);
                return;
            }

            // If we are adding files outside of the build directory root them in the directory that was added (incl. structure)
            CarbonDirectory fileRelativeRoot = this.context.BuildDir;
            if (!directory.StartsWith(this.context.BuildDir))
            {
                fileRelativeRoot = directory;
            }

            int filesAdded;
            if (recursive)
            {
                filesAdded = this.AddDirectorySourcesRecursive(directory, fileRelativeRoot, pattern);
            }
            else
            {
                filesAdded = this.AddDirectorySources(directory, fileRelativeRoot, pattern);
            }

            Logger.Info("Added {0} Sources", filesAdded);
        }

        private int AddDirectorySourcesRecursive(CarbonDirectory directory, CarbonDirectory fileRelativeRoot, string pattern)
        {
            int filesAdded = 0;
            CarbonDirectoryResult[] subDirectories = directory.GetDirectories();
            for (var i = 0; i < subDirectories.Length; i++)
            {
                CarbonFileResult[] projectFiles = subDirectories[i].Absolute.GetFiles("*.cbp");
                if (projectFiles.Length > 0)
                {
                    Logger.Info("Skipping AddSources for {0}, sub-project found ({1})", subDirectories[i].Relative, projectFiles[0]);
                    continue;
                }

                filesAdded += this.AddDirectorySourcesRecursive(subDirectories[i].Absolute, fileRelativeRoot, pattern);
            }

            filesAdded += this.AddDirectorySources(directory, fileRelativeRoot, pattern);
            return filesAdded;
        }

        private int AddDirectorySources(CarbonDirectory directory, CarbonDirectory fileRelativeRoot, string pattern)
        {
            CarbonFileResult[] matches = directory.GetFiles(pattern);
            foreach (CarbonFileResult result in matches)
            {
                this.context.AddSource(result.Absolute, result.Absolute.ToRelative<CarbonFile>(fileRelativeRoot));
            }

            return matches.Length;
        }

        public void AddSource(string path)
        {
            Logger.Info("Adding source {0}", path);
            var source = new CarbonFile(path);
            if (!source.Exists)
            {
                Logger.Warn("Skipping AddSource for {0}, does not exist", path);
                return;
            }

            this.context.AddSource(source, source); 
        }

        public void AddContent(string path, string pattern = "*.png", bool recursive = false)
        {
            Logger.Info("Adding content from {0}", path);
            var directory = new CarbonDirectory(path);
            if (!directory.Exists)
            {
                Logger.Warn("Skipping AddContent for {0}, does not exist", path);
                return;
            }

            CarbonFileResult[] matches = directory.GetFiles(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (CarbonFileResult result in matches)
            {
                this.context.AddContent(result.Absolute, result.Absolute.ToRelative<CarbonFile>(this.context.BuildDir));
            }

            Logger.Info("Added {0} Content Files", matches.Length);
        }

        public void AddConfig(string file)
        {
            this.context.AddConfig(new CarbonFile(file));
        }
        
        public void SetDefaultOutputPath(string path)
        {
            this.context.DefaultOutputPath = path;
        }

        public void SetDefaultIntermediateOutputPath(string path)
        {
            this.context.DefaultIntermediateOutputPath = path;
        }

        public void SetCodeAnalysisRules(string path)
        {
            this.context.CodeAnalysisRules = path;
        }

        public void SetFramework(string framework)
        {
            this.context.Framework = framework;
        }

        public void SetFrameworkProfile(string profile)
        {
            this.context.FrameworkProfile = profile;
        }

        public BuildConfigObject CreateBuildConfig(string name, string target)
        {
            BuildConfigObject config = this.context.AddBuildConfig(string.Concat(name, '_', target), name);
            if (config == null)
            {
                return null;
            }

            config.Target = target;
            return config;
        }

        public void SetOutputType(string typeString)
        {
            BuildTargetType type;
            if (!Enum.TryParse(typeString, out type))
            {
                Logger.Warn("Unknown Output type: {0}", typeString);
            }

            this.context.OutputType = type;
        }

        public void SetNamespace(string name)
        {
            this.context.Namespace = name;
        }

        public void SetIcon(string fileName)
        {
            this.context.Icon = fileName;
        }

        public void SetEntryPoint(string entry)
        {
            this.context.EntryPoint = entry;
        }

        public void AddStandardReferences()
        {
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystem);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemCore);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemData);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemXml);
        }

        public void AddReference(string name, string hintPath = null)
        {
            var reference = new BuildReference {Name = name};
            if (!string.IsNullOrEmpty(hintPath))
            {
                reference.HintPath = hintPath;
            }

            this.context.AddReference(reference);
        }

        public void AddProjectReference(string path, string nameSpace, bool isCSharp = true)
        {
            var reference = new BuildProjectReference {Path = path, Namespace = nameSpace, IsCSharpProject = isCSharp};
            this.context.AddProjectReference(reference);
        }

        public string WriteProjectFile()
        {
            YamlCrystalConfig yamlConfig = new YamlCrystalConfig();

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
            
            CarbonFile outputFile = this.context.BuildDir.ToFile(this.context.Namespace + SharpConstants.ProjectFileExtensionCSharp);
            var outData = CSPFile.Create();
            this.context.SaveAsProjectSettings(outData.AddPropertyGroup());

            CarbonFile yamlOutputFile = this.context.BuildDir.ToFile("build.cby");

            IList<BuildConfigObject> buildConfigs = this.context.BuildConfigs.Values.ToList();
            for (int i = 0; i < buildConfigs.Count; i++)
            {
                BuildConfigObject build = buildConfigs[i];
                if (string.IsNullOrEmpty(build.OutputPath))
                {
                    build.OutputPath = this.context.DefaultOutputPath;
                }

                if (string.IsNullOrEmpty(build.IntermediateOutputPath))
                {
                    build.IntermediateOutputPath = this.context.DefaultIntermediateOutputPath;
                }

                if (string.IsNullOrEmpty(build.CodeAnalysisRules))
                {
                    build.CodeAnalysisRules = this.context.CodeAnalysisRules;
                }

                build.SaveAsBuildSettings(outData.AddPropertyGroup());
            }

            foreach (CarbonFile source in this.context.Sources)
            {
                outData.AddItem("Compile", source.ToRelative<CarbonFile>(this.context.BuildDir).GetPath());
            }

            foreach (CarbonFile content in this.context.Content)
            {
                outData.AddItem("Resource", content.ToRelative<CarbonFile>(this.context.BuildDir).GetPath());
            }

            foreach (CarbonFile source in this.context.ConfigFiles)
            {
                outData.AddItem("None", source.ToRelative<CarbonFile>(this.context.BuildDir).GetPath());
            }

            foreach (CarbonFile source in this.context.TTSources.Keys)
            {
                CarbonFile target = this.context.TTSources[source];
                outData.AddTT(source.ToRelative<CarbonFile>(this.context.BuildDir), target.ToRelative<CarbonFile>(this.context.BuildDir));
            }

            foreach (CarbonFile source in this.context.XamlSources)
            {
                outData.AddXaml(source.ToRelative<CarbonFile>(this.context.BuildDir));
            }

            foreach (BuildReference reference in this.context.References)
            {
                yamlConfig.References.Add(new YamlReference { Id = reference.Name, Path = reference.HintPath });

                outData.AddReference(reference);
            }

            foreach (BuildProjectReference reference in this.context.ProjectReferences)
            {
                outData.AddProjectReference(reference);
            }

            if (!string.IsNullOrEmpty(this.context.Icon))
            {
                var iconGroup = outData.AddPropertyGroup();
                iconGroup.AddProperty("ApplicationIcon", this.context.Icon);

                outData.AddItem("Content", this.context.Icon);
            }

            if (!string.IsNullOrEmpty(this.context.EntryPoint))
            {
                var entryGroup = outData.AddPropertyGroup();
                entryGroup.AddProperty("StartupObject", this.context.EntryPoint);
            }

            CarbonFile packageFile = this.context.BuildDir.ToFile(SharpConstants.NugetPackageFile);
            if (packageFile.Exists)
            {
                outData.AddItem("None", packageFile.ToRelative<CarbonFile>(this.context.BuildDir).GetPath());
            }

            outData.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            
            outData.Save(outputFile);

            yamlConfig.Projects.Add(new YamlProject
                                        {
                                            Id = this.context.Namespace,
                                            CodeAnalysisRules = this.context.CodeAnalysisRules,
                                            Framework = this.context.Framework,
                                            FrameworkProfile = this.context.FrameworkProfile,
                                            IntermediateOutputPath = this.context.DefaultIntermediateOutputPath,
                                            OutputPath = this.context.DefaultOutputPath,
                                            Namespace = this.context.Namespace,
                                            TargetType = this.context.OutputType
                                        });

            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, yamlConfig);
                yamlOutputFile.WriteAsString(writer.ToString());
            }

            return outputFile.GetPath();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void LoadResourceScript()
        {
            // We do not have an attached resource script, skip
        }
    }
}
