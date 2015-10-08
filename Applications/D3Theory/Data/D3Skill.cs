namespace D3Theory.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonObject(MemberSerialization.OptOut)]
    public class D3Skill
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

        [DefaultValue(null)]
        public List<D3SkillRune> Runes { get; set; }

        public D3SkillRune GetRune(string name)
        {
            if (this.Runes == null)
            {
                return null;
            }

            return this.Runes.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

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
