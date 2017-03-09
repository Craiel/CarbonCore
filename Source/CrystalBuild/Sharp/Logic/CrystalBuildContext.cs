﻿namespace CarbonCore.CrystalBuild.Sharp.Logic
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Data.CSP;
    using Enums;

    using NLog;

    using Utils.IO;

    public class CrystalBuildContext
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CrystalBuildContext()
        {
            this.Guid = Guid.NewGuid();

            this.Sources = new List<CarbonFile>();
            this.Content = new List<CarbonFile>();
            this.ConfigFiles = new List<CarbonFile>();
            this.TTSources = new Dictionary<CarbonFile, CarbonFile>();
            this.XamlSources = new List<CarbonFile>();

            this.BuildConfigs = new Dictionary<string, BuildConfigObject>();

            this.References = new List<BuildReference>();
            this.ProjectReferences = new List<BuildProjectReference>();

            this.Reset();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Guid Guid { get; }

        public CarbonDirectory ProjectRoot { get; set; }

        public CarbonDirectory BuildDir { get; set; }

        public string Framework { get; set; }

        public string FrameworkProfile { get; set; }

        public string DefaultOutputPath { get; set; }

        public string DefaultIntermediateOutputPath { get; set; }

        public string CodeAnalysisRules { get; set; }

        public string Namespace { get; set; }

        public string Icon { get; set; }

        public string EntryPoint { get; set; }

        public BuildTargetType OutputType { get; set; }

        public IList<CarbonFile> Sources { get; }

        public IList<CarbonFile> Content { get; }

        public IList<CarbonFile> ConfigFiles { get; }

        public IDictionary<CarbonFile, CarbonFile> TTSources { get; }

        public IList<CarbonFile> XamlSources { get; }

        public IDictionary<string, BuildConfigObject> BuildConfigs { get; }

        public IList<BuildReference> References { get; }

        public IList<BuildProjectReference> ProjectReferences { get; private set; }

        public void AddSource(CarbonFile sourceFile, CarbonFile relativeSource)
        {
            CarbonFile ttScript = sourceFile.ChangeExtension(SharpConstants.ExtensionTT);
            if (ttScript.Exists)
            {
                // This source file is generated by a TT, mark it seperatly
                this.AddTTSource(ttScript, sourceFile);
                return;
            }

            if (sourceFile.FileName.EndsWith(".xaml.cs"))
            {
                CarbonFile xaml = sourceFile.GetDirectory().ToFile(sourceFile.FileNameWithoutExtension);
                this.AddXamlSource(xaml);
                return;
            }

            if (this.Sources.Contains(relativeSource))
            {
                Logger.Warn("Skipping Duplicate Source: {0}", relativeSource);
                return;
            }

            this.Sources.Add(relativeSource);
        }

        public void AddContent(CarbonFile source, CarbonFile relativeSource)
        {
            if (this.Content.Contains(relativeSource))
            {
                Logger.Warn("Skipping Duplicate Content: {0}", relativeSource);
                return;
            }

            this.Content.Add(relativeSource);
        }

        public void AddConfig(CarbonFile file)
        {
            this.ConfigFiles.Add(file);
        }

        public BuildConfigObject AddBuildConfig(string id, string name)
        {
            if (this.BuildConfigs.ContainsKey(id))
            {
                Logger.Error("Build configuration already exists: {0}", id);
                return null;
            }

            BuildConfigObject config = new BuildConfigObject(name);
            this.BuildConfigs.Add(id, config);
            return config;
        }

        public BuildConfigObject GetBuildConfig(string name)
        {
            BuildConfigObject config;
            if (this.BuildConfigs.TryGetValue(name, out config))
            {
                return config;
            }

            return null;
        }

        public void AddReference(BuildReference reference)
        {
            this.References.Add(reference);
        }

        public void AddProjectReference(BuildProjectReference reference)
        {
            this.ProjectReferences.Add(reference);
        }

        public void Reset()
        {
            this.DefaultOutputPath = "bin";
            this.DefaultIntermediateOutputPath = "obj";
            this.Framework = "4.5";
            this.FrameworkProfile = string.Empty;
            this.OutputType = BuildTargetType.Library;
            this.Sources.Clear();
        }

        public void SaveAsProjectSettings(CSPPropertyGroup target)
        {
            target.AddProperty("OutputType", Enum.GetName(typeof(BuildTargetType), this.OutputType));
            target.AddProperty("ProjectGuid", string.Concat("{", this.Guid.ToString().ToUpperInvariant(), "}"));
            target.AddProperty("AssemblyName", this.Namespace);
            target.AddProperty("RootNamespace", this.Namespace);
            target.AddProperty("TargetFrameworkVersion", this.Framework);
            target.AddProperty("TargetFrameworkProfile", this.FrameworkProfile);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AddTTSource(CarbonFile ttScript, CarbonFile targetFile)
        {
            if (this.TTSources.ContainsKey(ttScript))
            {
                Logger.Warn("Skipping Duplicate TT Source: {0}", ttScript);
                return;
            }

            this.TTSources.Add(ttScript, targetFile);
        }

        private void AddXamlSource(CarbonFile xaml)
        {
            if (this.XamlSources.Contains(xaml))
            {
                Logger.Warn("Skipping Duplicate Xaml Source: {0}", xaml);
                return;
            }

            this.XamlSources.Add(xaml);
        }
    }
}
