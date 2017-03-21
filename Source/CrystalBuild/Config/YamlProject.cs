namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    using Enums;
    
    public class YamlProject : YamlConfigBase
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlProject()
        {
            this.Sources = new List<YamlProjectSource>();
            this.References = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Namespace { get; set; }

        public BuildTargetType TargetType { get; set; }

        public string Framework { get; set; }

        public string FrameworkProfile { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }

        public string CodeAnalysisRules { get; set; }

        public IList<YamlProjectSource> Sources { get; set; }

        public IList<string> References { get; set; }
    }
}
