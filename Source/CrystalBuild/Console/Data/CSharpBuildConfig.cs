namespace CarbonCore.Applications.CrystalBuild.CSharp.Data
{
    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class CSharpBuildConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonIgnore]
        public CarbonDirectory WorkingDirectory { get; set; }
    }
}
