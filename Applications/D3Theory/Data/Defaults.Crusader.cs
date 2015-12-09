namespace CarbonCore.Modules.D3Theory.Data
{
    using System.Collections.Generic;

    public static class DefaultsCrusader
    {
        private static Dictionary<D3Attribute, float> attributes;
        private static List<D3Skill> skills;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Dictionary<D3Attribute, float> Attributes
        {
            get
            {
                return attributes ?? (attributes = GetAttributes());
            }
        }

        public static List<D3Skill> Skills
        {
            get
            {
                return skills ?? (skills = GetSkills());
            }
        }

        private static Dictionary<D3Attribute, float> GetAttributes()
        {
            return new Dictionary<D3Attribute, float>
                       {
                           { D3Attribute.LifePerVit, 80.0f },
                           { D3Attribute.AttackSpeed, 1.0f },
                           { D3Attribute.CritRate, 5.0f },
                           { D3Attribute.CritDmg, 50.0f },
                       };
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static List<D3Skill> GetSkills()
        {
            return null;
        }
    }
}
