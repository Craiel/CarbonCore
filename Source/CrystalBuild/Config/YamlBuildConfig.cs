namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    public class YamlBuildConfig : YamlConfigBase
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlBuildConfig()
        {
            this.Defines = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Platform { get; set; }

        public IList<string> Defines { get; set; }

        public bool Optimize { get; set; }
    }
}