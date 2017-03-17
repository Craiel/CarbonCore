namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    public class YamlCrystalConfig
    {
        public YamlCrystalConfig()
        {
            this.BuildConfigs = new List<YamlBuildConfig>();
            this.References = new List<YamlReference>();
            this.Projects = new List<YamlProject>();
        }

        public IList<YamlBuildConfig> BuildConfigs { get; set; }

        public IList<YamlReference> References { get; set; }

        public IList<YamlProject> Projects { get; set; }
    }
}
