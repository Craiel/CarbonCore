namespace CarbonCore.CrystalBuild.Logic
{
    using System.Collections.Generic;
    
    using CarbonCore.Utils.IO;

    using Config;

    public class BuildContext
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildContext()
        {
            this.ConfigFiles = new List<CarbonFile>();
            this.ConfigCache = new Dictionary<CarbonFile, YamlCrystalConfig>();
            this.Variables = new Dictionary<string, string>();
            this.BuildConfigs = new Dictionary<string, YamlBuildConfig>();
            this.References = new Dictionary<string, YamlReference>();
            this.Projects = new Dictionary<string, YamlProject>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<CarbonFile> ConfigFiles { get; private set; }

        public IDictionary<CarbonFile, YamlCrystalConfig> ConfigCache { get; private set; }

        public IDictionary<string, string> Variables { get; private set; }

        public IDictionary<string, YamlBuildConfig> BuildConfigs { get; private set; }

        public IDictionary<string, YamlReference> References { get; private set; }

        public IDictionary<string, YamlProject> Projects { get; private set; }

        public CarbonDirectory Root { get; set; }
    }
}
