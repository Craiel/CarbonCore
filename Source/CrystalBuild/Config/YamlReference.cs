namespace CarbonCore.CrystalBuild.Config
{
    public class YamlReference : YamlConfigBase
    {
        public string NuGetId { get; set; }

        public string Version { get; set; }

        public string TargetFramework { get; set; }

        public string Path { get; set; }

        public bool NuGet { get; set; }
    }
}
