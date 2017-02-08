namespace CarbonCore.CrystalBuild.Sharp.Logic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Data;
    using Data.CSP;
    using Enums;
    using Utils.Diagnostics;
    using Utils.IO;
    using Utils.Lua.Contracts;
    using Utils.Lua.Logic.Library;

    public class LuaLibraryCrystalBuildConfig : LuaLibraryBase
    {
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

            this.AddLibraryFunction<string>(this.SetDefaultOutputPath);
            this.AddLibraryFunction<string>(this.SetDefaultIntermediateOutputPath);
            this.AddLibraryFunction<string>(this.SetCodeAnalysisRules);
            this.AddLibraryFunction<string>(this.SetFramework);
            this.AddLibraryFunction<string>(this.SetFrameworkProfile);
            this.AddLibraryFunction<string>(this.SetOutputType);
            this.AddLibraryFunction<string>(this.SetNamespace);

            this.AddLibraryFunction(this.AddStandardReferences);
            this.AddLibraryFunction<string, string>(this.AddReference);
            this.AddLibraryFunction<string, string>(this.AddProjectReference);

            this.AddLibraryFunction<string, string, BuildConfigObject>(this.CreateBuildConfig);

            this.AddLibraryFunction(this.WriteProjectFile);
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void AddSources(string path, string pattern = "*.cs", bool recursive = false)
        {
            Diagnostic.Info("Adding sources from {0}", path);
            var directory = new CarbonDirectory(path);
            if (!directory.Exists)
            {
                Diagnostic.Warning("Skipping AddSources for {0}, does not exist", path);
                return;
            }

            CarbonFileResult[] matches = directory.GetFiles(pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (CarbonFileResult result in matches)
            {
                this.context.AddSource(result.Absolute, result.Absolute.ToRelative<CarbonFile>(this.context.BuildDir));
            }

            Diagnostic.Info("Added {0} Sources", matches.Length);
        }

        public void AddSource(string path)
        {
            Diagnostic.Info("Adding source {0}", path);
            var source = new CarbonFile(path);
            if (!source.Exists)
            {
                Diagnostic.Warning("Skipping AddSource for {0}, does not exist", path);
                return;
            }

            this.context.AddSource(source, source); 
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
            BuildConfigObject config = this.context.AddBuildConfig(name);
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
                Diagnostic.Warning("Unknown Output type: {0}", typeString);
            }

            this.context.OutputType = type;
        }

        public void SetNamespace(string name)
        {
            this.context.Namespace = name;
        }

        public void AddStandardReferences()
        {
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystem);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemCore);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemData);
            this.context.AddReference(DefaultBuildReferences.DefaultReferenceSystemXml);
        }

        public void AddReference(string name, string hintPath)
        {
            var reference = new BuildReference {Name = name};
            if (!string.IsNullOrEmpty(hintPath))
            {
                reference.HintPath = hintPath;
            }

            this.context.AddReference(reference);
        }

        public void AddProjectReference(string path, string nameSpace)
        {
            var reference = new BuildProjectReference {Path = path, Namespace = nameSpace};
            this.context.AddProjectReference(reference);
        }

        public string WriteProjectFile()
        {
            CarbonFile outputFile = this.context.BuildDir.ToFile(this.context.Namespace + SharpConstants.ProjectFileExtension);
            var outData = CSPFile.Create();
            this.context.SaveAsProjectSettings(outData.AddPropertyGroup());

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

            foreach (CarbonFile source in this.context.TTSources)
            {
                outData.AddTT(source.ToRelative<CarbonFile>(this.context.BuildDir));
            }

            foreach (BuildReference reference in this.context.References)
            {
                outData.AddReference(reference);
            }

            foreach (BuildProjectReference reference in this.context.ProjectReferences)
            {
                outData.AddProjectReference(reference);
            }

            CarbonFile packageFile = this.context.BuildDir.ToFile(SharpConstants.NugetPackageFile);
            if (packageFile.Exists)
            {
                outData.AddItem("None", packageFile.ToRelative<CarbonFile>(this.context.BuildDir).GetPath());
            }

            outData.AddImport(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
            
            outData.Save(outputFile);

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
