namespace CarbonCore.CrystalBuild.Sharp.Data
{
    using System.Collections.Generic;
    using CSP;
    using Utils;
    using Utils.Diagnostics;

    public class BuildConfigObject
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildConfigObject(string name)
        {
            this.EnabledTargets = new List<string>();
            this.Defines = new List<string>();

            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool Optimize { get; set; }

        public IList<string> EnabledTargets { get; private set; }

        public IList<string> Defines { get; }

        public string Name { get; }

        public string Target { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }
        
        public string CodeAnalysisRules { get; set; }

        public void SetDefine(string define)
        {
            if (this.Defines.Contains(define))
            {
                Diagnostic.Warning("Define {0} already set", define);
                return;
            }

            this.Defines.Add(define);
        }

        public void UnsetDefine(string define)
        {
            if (!this.Defines.Contains(define))
            {
                Diagnostic.Warning("Define is not set: {0}", define);
                return;
            }

            this.Defines.Remove(define);
        }

        public void SetOutputPath(string newPath)
        {
            this.OutputPath = newPath;
        }

        public void SetIntermediateOutputPath(string newPath)
        {
            this.IntermediateOutputPath = newPath;
        }

        public void SetOptimize(bool value)
        {
            this.Optimize = value;
        }

        public void SaveAsBuildSettings(CSPPropertyGroup target)
        {
            target.Condition = $"'$(Configuration)|$(Platform)' == '{this.Name}|{this.Target}'";
            target.AddProperty("Optimize", this.Optimize.ToString());
            target.AddProperty("OutputPath", this.OutputPath.GetAgnosticPath());
            target.AddProperty("IntermediateOutputPath", this.IntermediateOutputPath.GetAgnosticPath());
            target.AddProperty("DefineConstants", string.Join(";", this.Defines));
            target.AddProperty("PlatformTarget", this.Target);
            target.AddProperty("CodeAnalysisRuleSet", this.CodeAnalysisRules.GetAgnosticPath());
        }

        public void EnableForTarget(string target)
        {
            this.EnabledTargets.Add(target);
        }
    }
}
