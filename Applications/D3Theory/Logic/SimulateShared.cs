namespace CarbonCore.Modules.D3Theory.Logic
{
    using System.Collections.Generic;

    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;

    public static class SimulateShared
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static float GetAttackSpeed(ISimulationData data)
        {
            float speed = data.AttributeSet.GetValue(D3Attribute.AttackSpeed);
            float speedOffhand = data.AttributeSet.GetValue(D3Attribute.AttackSpeedOffhand);
            if (speedOffhand >= 0)
            {
                // Taken from the interweb, 15% bonus for dual wield then cut in half
                return ((speed * 1.15f) + (speedOffhand * 1.15f)) / 2.0f;
            }

            return 1.0f / speed;
        }

        public static float GetActionTime(ISimulationData data)
        {
            float speed = GetAttackSpeed(data);
            return data.MainData.Generic.ActionDelay + speed;
        }

        public static bool CanExecuteAction(ISimulationData data)
        {
            float timeRequired = GetActionTime(data);
            if (data.CurrentTime + timeRequired >= data.MaxTime)
            {
                return false;
            }

            return true;
        }

        public static void FinalizeAction(ISimulationData data, SimulationSampleSet sampleSet)
        {
            float actionTime = GetActionTime(data);
            data.CurrentTime += actionTime;
            sampleSet.AddStat(SimulationStat.ActionTime, actionTime);
            sampleSet.AddStat(SimulationStat.DelayTime, data.MainData.Generic.ActionDelay);
            sampleSet.AddStat(SimulationStat.Actions);
        }

        public static void DeductSkillInitialCost(ISimulationData data)
        {
            float primaryCost = data.SelectedSkill.GetValue(D3SkillAttribute.PrimaryResourceDrainInitially);
            float secondaryCost = data.SelectedSkill.GetValue(D3SkillAttribute.SecondaryResourceDrainInitially);
            float deduction = 1.0f - (data.AttributeSet.GetValue(D3Attribute.ResourceCostReduce) / 100.0f);
            data.Character.RemoveValue(D3EntityAttribute.PrimaryResource, primaryCost * deduction);
            data.Character.RemoveValue(D3EntityAttribute.SecondaryResource, secondaryCost * deduction);
        }

        public static void DeductSkillCost(ISimulationData data)
        {
            float primaryCost = data.SelectedSkill.GetValue(D3SkillAttribute.PrimaryResourceDrainPerAction);
            float secondaryCost = data.SelectedSkill.GetValue(D3SkillAttribute.SecondaryResourceDrainPerAction);
            float deduction = 1.0f - (data.AttributeSet.GetValue(D3Attribute.ResourceCostReduce) / 100.0f);
            data.Character.RemoveValue(D3EntityAttribute.PrimaryResource, primaryCost * deduction);
            data.Character.RemoveValue(D3EntityAttribute.SecondaryResource, secondaryCost * deduction);
        }

        public static bool CheckForCritical(ISimulationData data)
        {
            float rate = data.AttributeSet.GetValue(D3Attribute.CritRate) / 100.0f;
            return data.Random() <= rate;
        }

        public static bool ApplyCritical(ISimulationData data, ref float min, ref float max)
        {
            if (!CheckForCritical(data))
            {
                return false;
            }

            float critDmgBase = data.AttributeSet.GetValue(D3Attribute.CritDmg)
                            + data.AttributeSet.GetValue(D3Attribute.GemCritDmg);
            float multiplier = critDmgBase / 100.0f;
            min *= multiplier;
            max *= multiplier;
            return true;
        }

        public static void ApplyBaseAttack(ISimulationData data, SimulationSampleSet sampleSet, IEntity target)
        {
            float min = GetMinDamage(data);
            float max = GetMaxDamage(data);

            bool isCritical = ApplyCritical(data, ref min, ref max);

            float damage = data.Random(min, max);
            ApplyDamage(data, target, damage);
            sampleSet.AddDamage(D3DamageType.Physical, damage, isCritical);
        }
        
        public static int GetSkillTargetCount(ISimulationData data)
        {
            var max = (int)data.SelectedSkill.GetValue(D3SkillAttribute.TargetCountMax);
            if (max <= 0)
            {
                max = int.MaxValue;
            }

            return max;
        }

        public static void ApplySkillAttack(ISimulationData data, SimulationSampleSet sampleSet, IEntity target, bool isPrimary)
        {
            float skillMultiplier = data.SelectedSkill.GetValue(D3SkillAttribute.DmgBonusPrimary) / 100.0f;
            if (!isPrimary)
            {
                float secondaryMultiplier = data.SelectedSkill.GetValue(D3SkillAttribute.DmgBonusSecondary) / 100.0f;
                if (secondaryMultiplier > 0.0f)
                {
                    skillMultiplier = secondaryMultiplier;
                }
            }

            float min = GetMinDamage(data) * skillMultiplier;
            float max = GetMaxDamage(data) * skillMultiplier;

            bool isCritical = ApplyCritical(data, ref min, ref max);

            float damage = data.Random(min, max);
            ApplyDamage(data, target, damage);
            sampleSet.AddDamage(D3DamageType.Physical, damage, isCritical);
        }

        public static void ApplySkillRunning(ISimulationData data)
        {
            float duration = data.SelectedSkill.GetValue(D3SkillAttribute.Duration);
            if (duration <= 0.0f)
            {
                return;
            }

            data.SetSkillRunning(data.SelectedSkill, data.CurrentTime + duration);
        }

        public static void ApplySkillCooldown(ISimulationData data)
        {
            float coolDown = data.SelectedSkill.GetValue(D3SkillAttribute.Cooldown);
            if (coolDown <= 0.0f)
            {
                return;
            }

            data.SetSkillCooldown(data.SelectedSkill, data.CurrentTime + coolDown);
        }

        public static void ApplyDamage(ISimulationData data, IEntity target, float value)
        {
            // Todo: Mitigation
            target.RemoveValue(D3EntityAttribute.Life, value);
        }
        
        public static AttributeSet GetSkillAttributes(ISimulationData data, string skillName, string runeName = null)
        {
            data.SelectSkill(skillName, runeName);
            if (data.SelectedSkill == null)
            {
                return null;
            }

            var set = new AttributeSet(data.Class, initializeWithClass: false)
                          {
                              DamageType = data.SelectedSkill.GetDamageType()
                          };

            data.SelectedSkill.MergeAttributes(set);

            return set;
        }

        public static float GetMinDamage(ISimulationData data)
        {
            float multiplier = data.AttributeSet.GetValue(D3Attribute.DmgIncreasePrimary) / 100;
            float baseDmg = data.AttributeSet.GetValue(D3Attribute.DmgMin);
            baseDmg += data.AttributeSet.GetValue(D3Attribute.GemDmg);
            return baseDmg * multiplier;
        }

        public static float GetMaxDamage(ISimulationData data)
        {
            float multiplier = data.AttributeSet.GetValue(D3Attribute.DmgIncreasePrimary) / 100;
            float baseDmg = data.AttributeSet.GetValue(D3Attribute.DmgMax);
            baseDmg += data.AttributeSet.GetValue(D3Attribute.GemDmg);
            return baseDmg * multiplier;
        }

        public static bool CheckSufficientResource(ISimulationData data)
        {
            float primary = data.Character.GetValue(D3EntityAttribute.PrimaryResource);
            float secondary = data.Character.GetValue(D3EntityAttribute.SecondaryResource);

            float primaryDrain = data.SelectedSkill.GetValue(D3SkillAttribute.PrimaryResourceDrainPerAction);
            float secondaryDrain = data.SelectedSkill.GetValue(D3SkillAttribute.SecondaryResourceDrainPerAction);

            return primaryDrain <= primary && secondaryDrain <= secondary;
        }

        public static void ApplyRegen(ISimulationData data, SimulationSampleSet sampleSet)
        {
            // Resources
            float primaryMax = data.AttributeSet.GetValue(D3Attribute.PrimaryResource);
            float secondaryMax = data.AttributeSet.GetValue(D3Attribute.SecondaryResource);

            float primaryRegen = data.AttributeSet.GetValue(D3Attribute.PrimaryResourceRegen);
            float secondaryRegen = data.AttributeSet.GetValue(D3Attribute.SecondaryResourceRegen);

            primaryRegen = data.Character.AddValue(D3EntityAttribute.PrimaryResource, primaryRegen, primaryMax);
            secondaryRegen = data.Character.AddValue(D3EntityAttribute.SecondaryResource, secondaryRegen, secondaryMax);
            sampleSet.AddStat(SimulationStat.PrimaryResourceRegen, primaryRegen);
            sampleSet.AddStat(SimulationStat.SecondaryResourceRegen, secondaryRegen);

            // Life
            float lifeMax = data.AttributeSet.GetValue(D3Attribute.Life);
            float lifeRegen = data.AttributeSet.GetValue(D3Attribute.LifeRegen);

            lifeRegen = data.Character.AddValue(D3EntityAttribute.Life, lifeRegen, lifeMax);
            sampleSet.AddStat(SimulationStat.LifeRegen, lifeRegen);
        }

        public static void ApplyRunningSkills(ISimulationData data, SimulationSampleSet set)
        {
            // Save the selection to restore after we are done
            SkillCombo currentSkill = data.SelectedSkill;
            IList<SkillCombo> runningSkills = data.GetRunningSkills();
            foreach (SkillCombo skill in runningSkills)
            {
                if (data.CurrentTime - skill.LastTrigger > 1.0f)
                {
                    // Select the skill and execute it
                    data.SelectedSkill = skill;
                    ExecuteSkillRunning(data, set);
                    set.AddStat(SimulationStat.RunningExecutions);
                }
            }

            data.SelectedSkill = currentSkill;
        }

        // Similar to ExecuteSkill but for running effects
        public static void ExecuteSkillRunning(ISimulationData data, SimulationSampleSet set)
        {
            int targetMax = GetSkillTargetCount(data);
            IList<IEntity> targets = data.PickRandomTargets(targetMax);
            for (int i = 0; i < targets.Count; i++)
            {
                IEntity target = targets[i];
                ApplySkillAttack(data, set, target, i == 0);
            }

            data.SelectedSkill.LastTrigger = data.CurrentTime;
        }

        public static ExecuteSkillResult ExecuteSkill(ISimulationData data, SimulationSampleSet set)
        {
            // try to perform action
            if (data.IsRunning(data.SelectedSkill))
            {
                return ExecuteSkillResult.Running;
            }

            if (data.IsOnCooldown(data.SelectedSkill))
            {
                return ExecuteSkillResult.Cooldown;
            }

            if (!CheckSufficientResource(data))
            {
                return ExecuteSkillResult.InsufficientResources;
            }

            if (!CanExecuteAction(data))
            {
                return ExecuteSkillResult.InsufficientTime;
            }

            int targetMax = GetSkillTargetCount(data);
            IList<IEntity> targets = data.PickRandomTargets(targetMax);
            for (int i = 0; i < targets.Count; i++)
            {
                IEntity target = targets[i];
                ApplySkillAttack(data, set, target, i == 0);
            }

            data.SelectedSkill.LastTrigger = data.CurrentTime;

            ApplySkillCooldown(data);
            ApplySkillRunning(data);

            // Cost is only once per action
            DeductSkillCost(data);
            FinalizeAction(data, set);
            return ExecuteSkillResult.Success;
        }
    }
}
