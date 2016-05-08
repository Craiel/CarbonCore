namespace CarbonCore.CrystalBuild.Data
{
    using System.Collections.Generic;

    using CarbonCore.CrystalBuild.Logic.Enums;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class BuildProjectConfiguration
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildProjectConfiguration()
        {
            this.BuildConfigurations = new List<BuildConfiguration>();
            this.References = new List<BuildReference>();
            this.SourceMapping = new Dictionary<CarbonDirectoryFilter, CarbonDirectory>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BuildTargetType Type { get; set; }

        public string TargetFileName { get; set; }

        public string NameSpace { get; set; }

        public string AssemblyName { get; set; }

        public string TargetFramework { get; set; }

        public string TargetFrameworkProfile { get; set; }

        public string PreBuildEvent { get; set; }

        public string PostBuildEvent { get; set; }

        public IList<BuildConfiguration> BuildConfigurations { get; set; }

        public IList<BuildReference> References { get; set; }

        [JsonConverter(typeof(JsonDictionaryConverter<CarbonDirectoryFilter, CarbonDirectory>))]
        public IDictionary<CarbonDirectoryFilter, CarbonDirectory> SourceMapping { get; set; }
    }
}
