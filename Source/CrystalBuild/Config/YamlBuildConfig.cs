namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    public class YamlBuildConfig : YamlConfigBase
    {
        public YamlBuildConfig()
        {
            this.Defines = new List<string>();
        }

        public string Platform { get; set; }

        public IList<string> Defines { get; set; }

        public bool Optimize { get; set; }
    }
}