namespace CarbonCore.CrystalBuild.Data
{
    using System.Collections.Generic;

    using CarbonCore.CrystalBuild.Logic.Enums;

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
        public string Id { get; set; }

        public string ParentConfigId { get; set; }

        public BuildTargetType Type { get; set; }

        public string Name { get; set; }

        public string NameSpace { get; set; }

        public string AssemblyName { get; set; }

        public string TargetFramework { get; set; }

        public IList<string> Defines { get; set; }
    }
}
