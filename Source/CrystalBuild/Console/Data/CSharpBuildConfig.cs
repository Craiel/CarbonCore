namespace CarbonCore.CrystalBuild.Console.Data
{
    using Newtonsoft.Json;

    using Utils.IO;

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
