namespace D3Theory.Data
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.IO;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class Simulation
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonIgnore]
        public CarbonFile File { get; set; }

        public int Seconds { get; set; }

        public int Level { get; set; }

        public int ParagonLevel { get; set; }

        public int TargetCountMin { get; set; }

        public int TargetCountMax { get; set; }

        public string Class { get; set; }

        public Dictionary<D3Attribute, float> Attributes { get; set; }

        public List<D3Gear> Gear { get; set; }
    }
}
