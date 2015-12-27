namespace CarbonCore.Applications.MCSync.Launcher.Logic
{
    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;
    
    [JsonObject(MemberSerialization.OptOut)]
    public class LaunchConfig
    {
        public CarbonDirectory Root { get; set; }
        
        public string SourcePath { get; set; }

        public string TargetPath { get; set; }

        public bool Force { get; set; }

        public bool Server { get; set; }
    }
}
