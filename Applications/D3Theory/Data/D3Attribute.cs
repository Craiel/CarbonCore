namespace CarbonCore.Modules.D3Theory.Data
{
    public enum D3Attribute
    {
        Undefined,

        // Internal
        LifePerVit,
        

        // Primary Attributes
        Str,
        Int,
        Dex,
        Vit,

        PrimaryResource,
        PrimaryResourceRegen,
        SecondaryResource,
        SecondaryResourceRegen,

        Life,
        LifeBonus,
        LifeRegen,
        LifeHit,
        LifeKill,

        DmgMin,
        DmgMax,
        DmgBonus,
        DmgBonusPhysical,
        DmgBonusFire,
        DmgBonusArcane,
        DmgBonusCold,
        DmgBonusLightning,
        DmgBonusPoison,

        DmgBonusDemons,

        AttackSpeed,
        AttackSpeedOffhand,
        AttackSpeedBonus,

        Armor,
        DmgIncreasePrimary,
        CritRate,
        CritDmg,
        DodgeRate,

        ResistAll,
        ResistPhysical,
        ResistFire,
        ResistArcane,
        ResistCold,
        ResistLightning,
        ResistPoison,

        Thorns,
        BonusExperience,
        ExtraGold,
        MagicFind,
        PickupRadius,
        HealBonus,
        MovementSpeed,
        Indestructable,
        BlindRate,
        ResourceCostReduce,

        // Gems
        GemStr,
        GemDex,
        GemInt,
        GemVit,
        GemDmg,
        GemCritDmg,

        // Skills DH
        SkillBonusDHCluster,
        SkillBonusDHRainOfVengeance,
        SkillBonusDHGenericGrenade,

        // Skills BA
        SkillBonusBAOverpower,
        SkillBonusBAWhirlwind
    }
}
