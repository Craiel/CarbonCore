namespace CarbonCore.CrystalBuild.Data
{
    using System.Collections.Generic;

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
        public IList<BuildConfiguration> BuildConfigurations { get; set; }

        public IList<BuildReference> References { get; set; }

        [JsonConverter(typeof(JsonDictionaryConverter<CarbonDirectoryFilter, CarbonDirectory>))]
        public IDictionary<CarbonDirectoryFilter, CarbonDirectory> SourceMapping { get; set; }
    }
}
