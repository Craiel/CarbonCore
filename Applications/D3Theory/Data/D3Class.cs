namespace D3Theory.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonObject(MemberSerialization.OptOut)]
    public class D3Class
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DefaultValue(null)]
        public string Name { get; set; }

        [DefaultValue(null)]
        public Dictionary<D3Attribute, float> Attributes { get; set; }

        [DefaultValue(D3Attribute.Undefined)]
        [JsonConverter(typeof(StringEnumConverter))]
        public D3Attribute PrimaryAttribute { get; set; }

        [DefaultValue(null)]
        public string PrimaryResourceName { get; set; }

        [DefaultValue(null)]
        public string SecondaryResourceName { get; set; }
        
        [DefaultValue(null)]
        public List<D3Skill> Skills { get; set; }

        public D3Skill GetSkill(string name)
        {
            if (this.Skills == null)
            {
                return null;
            }

            return this.Skills.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
