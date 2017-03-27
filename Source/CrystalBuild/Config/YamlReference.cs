namespace CarbonCore.CrystalBuild.Config
{
    public class YamlReference : YamlConfigBase
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string NuGetId { get; set; }

        public string Version { get; set; }

        public string[] TargetFrameworks { get; set; }

        public string Path { get; set; }

        public string[] Files { get; set; }

        public bool NuGet { get; set; }

        public YamlReference[] Children { get; set; }
    }
}
