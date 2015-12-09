namespace CarbonCore.Modules.D3Theory.Data
{
    using System.Collections.Generic;

    public static class DefaultsDemonHunter
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
                           { D3Attribute.PrimaryResource, 125.0f },
                           { D3Attribute.PrimaryResourceRegen, 4.0f },
                           { D3Attribute.SecondaryResource, 30.0f },
                           { D3Attribute.SecondaryResourceRegen, 1.0f },
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
            return new List<D3Skill>
                       {
                           new D3Skill
                               {
                                   Name = "Hungering Arrow",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 155.0f },
                                               { D3SkillAttribute.PrimaryResourcePerAction, 3.0f },
                                               { D3SkillAttribute.TargetCountMax, 1.0f },
                                               // Todo: 25% pierce
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Entangling Shot",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 200.0f },
                                               { D3SkillAttribute.PrimaryResourcePerAction, 3.0f },
                                               { D3SkillAttribute.TargetCountMax, 1.0f },
                                               // Todo: 60% slow on 3 enemies
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Bolas",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 160.0f },
                                               { D3SkillAttribute.DmgBonusSecondary, 110.0f },
                                               { D3SkillAttribute.PrimaryResourcePerAction, 3.0f },
                                               { D3SkillAttribute.TargetCountMax, 6.0f },
                                               // Todo: Check area, 14yards
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Impale",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 620.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainPerAction, 20.0f },
                                               { D3SkillAttribute.TargetCountMax, 1.0f },
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Strafe",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 400.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainPerAction, 12.0f },
                                               { D3SkillAttribute.TargetCountMax, 3.0f },
                                               // Todo: Target count needs update
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Multishot",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 360.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainPerAction, 25.0f },
                                               // Todo: Cone
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Rapid Fire",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 525.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainInitially, 20.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainPerAction, 6.0f },
                                               { D3SkillAttribute.TargetCountMax, 1.0f },
                                               // Todo: Check the channeling speed
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Chakram",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 380.0f },
                                               { D3SkillAttribute.PrimaryResourceDrainPerAction, 10.0f },
                                               { D3SkillAttribute.TargetCountMax, 2.0f },
                                           }
                               },
                           new D3Skill
                               {
                                   Name = "Rain of Vengeance",
                                   SkillAttributes =
                                       new Dictionary<D3SkillAttribute, float>
                                           {
                                               { D3SkillAttribute.DmgBonusPrimary, 1250.0f },
                                               { D3SkillAttribute.Cooldown, 30.0f },
                                               { D3SkillAttribute.Duration, 5.0f },
                                           }
                               }
                       };
        }
    }
}
