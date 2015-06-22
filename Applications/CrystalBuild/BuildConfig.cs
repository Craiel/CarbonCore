namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.Applications.CrystalBuild.Logic;
    using CarbonCore.Utils.Compat.IO;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonObject(MemberSerialization.OptOut)]
    public class BuildConfig
    {
        [JsonIgnore]
        public CarbonDirectory ProjectRoot { get; set; }

        public string Name { get; set; }

        public bool ExportSourceAsModule { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public BuildTargetPlatform TargetPlatform { get; set; }

        public CarbonDirectoryFilter[] Data { get; set; }
        public CarbonDirectoryFilter[] Templates { get; set; }
        public CarbonDirectoryFilter[] Sources { get; set; }
        public CarbonDirectoryFilter[] StyleSheets { get; set; }
        public CarbonDirectoryFilter[] Contents { get; set; }
        public CarbonDirectoryFilter[] Images { get; set; }

        public CarbonFile SourceTarget { get; set; }
        public CarbonFile SourceMain { get; set; }
        public CarbonFile TemplateTarget { get; set; }
        public CarbonFile DataTarget { get; set; }
        public CarbonFile StyleSheetTarget { get; set; }
        public CarbonDirectory ImageRoot { get; set; }
        public CarbonDirectory ContentTarget { get; set; }
    }
}
