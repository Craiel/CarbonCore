namespace CarbonCore.Modules.D3Theory.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonObject(MemberSerialization.OptOut)]
    public class D3Gear
    {
        public D3Gear()
        {
            this.Enabled = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DefaultValue(true)]
        public bool Enabled { get; set; }

        [DefaultValue(null)]
        public string Name { get; set; }

        [DefaultValue(D3DamageType.Undefined)]
        [JsonConverter(typeof(StringEnumConverter))]
        public D3DamageType DamageType { get; set; }

        [DefaultValue(D3GearType.Undefined)]
        [JsonConverter(typeof(StringEnumConverter))]
        public D3GearType Type { get; set; }

        [DefaultValue(null)]
        public Dictionary<D3Attribute, float> Attributes { get; set; }
    }
}
