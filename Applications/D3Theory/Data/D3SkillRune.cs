namespace CarbonCore.Modules.D3Theory.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonObject(MemberSerialization.OptOut)]
    public class D3SkillRune
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DefaultValue(null)]
        public string Name { get; set; }

        [DefaultValue(null)]
        public IDictionary<D3Attribute, float> Attributes { get; set; }

        [DefaultValue(null)]
        public IDictionary<D3SkillAttribute, float> SkillAttributes { get; set; }

        [DefaultValue(D3DamageType.Undefined)]
        [JsonConverter(typeof(StringEnumConverter))]
        public D3DamageType DamageType { get; set; }

        public float GetValue(D3SkillAttribute attribute)
        {
            if (this.SkillAttributes == null || !this.SkillAttributes.ContainsKey(attribute))
            {
                return 0.0f;
            }

            return this.SkillAttributes[attribute];
        }
    }
}
