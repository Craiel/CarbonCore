namespace CarbonCore.CrystalBuild.Data
{
    using System.Collections.Generic;
    
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class BuildConfiguration
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildConfiguration()
        {
            this.Defines = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public string OutputPath { get; set; }

        public string IntermediateOutputPath { get; set; }

        public string PlatformTarget { get; set; }

        public string RulesetFile { get; set; }

        public int LanguageVersion { get; set; }

        public IList<string> Defines { get; set; }
    }
}
