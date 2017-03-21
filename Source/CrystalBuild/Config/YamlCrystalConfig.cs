namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    public class YamlCrystalConfig
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlCrystalConfig()
        {
            this.Variables = new Dictionary<string, string>();
            this.BuildConfigs = new List<YamlBuildConfig>();
            this.References = new List<YamlReference>();
            this.Projects = new List<YamlProject>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IDictionary<string, string> Variables { get; set; }

        public IList<YamlBuildConfig> BuildConfigs { get; set; }

        public IList<YamlReference> References { get; set; }

        public IList<YamlProject> Projects { get; set; }
    }
}
