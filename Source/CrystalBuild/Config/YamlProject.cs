namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    using Sharp.Enums;

    public class YamlProject : YamlConfigBase
    {
        public YamlProject()
        {
            this.Sources = new List<YamlProjectSource>();
        }

        public string Namespace { get; set; }

        public BuildTargetType TargetType { get; set; }

        public string Framework { get; set; }

        public string FrameworkProfile { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }

        public string CodeAnalysisRules { get; set; }

        public IList<YamlProjectSource> Sources { get; set; }
    }
}
