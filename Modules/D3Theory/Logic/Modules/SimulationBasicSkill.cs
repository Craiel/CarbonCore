namespace CarbonCore.Modules.D3Theory.Logic.Modules
{
    using System;

    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;

    public class SimulationBasicSkill : ISimulationModule
    {
        private const float TestInterval = 0.1f;

        private readonly string skillName;
        private readonly string runeName;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SimulationBasicSkill(string skill, string rune = null)
        {
            if (string.IsNullOrWhiteSpace(skill))
            {
                throw new ArgumentException("Skill can not be empty");
            }

            this.skillName = skill;
            this.runeName = rune;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name
        {
            get
            {
                return string.Format("BasicSkill: {0}({1})", this.skillName, this.runeName ?? "None");
            }
        }

        public SimulationSampleSet Simulate(ISimulationData data)
        {
            AttributeSet skillAttributes = SimulateShared.GetSkillAttributes(data, this.skillName, this.runeName);
            if (skillAttributes == null)
            {
                return null;
            }

            var set = new SimulationSampleSet { Name = this.Name };

            SimulateShared.DeductSkillInitialCost(data);
            float lastRegen = 0;
            while (data.CurrentTime < data.MaxTime)
            {
                // Apply Regeneration
                while (data.CurrentTime - lastRegen > 1.0f)
                {
                    SimulateShared.ApplyRegen(data, set);
                    lastRegen += 1.0f;
                }

                // Update the targets and running skills
                data.UpdateTargets(set);
                data.UpdateSkills();

                // Process running skills
                SimulateShared.ApplyRunningSkills(data, set);

                // Check if we have enough resources to perform the action
                ExecuteSkillResult result = SimulateShared.ExecuteSkill(data, set);
                switch (result)
                {
                    case ExecuteSkillResult.Cooldown:
                        {
                            set.AddStat(SimulationStat.DelayCooldown, TestInterval);
                            break;
                        }

                    case ExecuteSkillResult.Running:
                        {
                            set.AddStat(SimulationStat.DelayRunning, TestInterval);
                            break;
                        }

                    case ExecuteSkillResult.InsufficientResources:
                        {
                            set.AddStat(SimulationStat.DelayResource, TestInterval);
                            break;
                        }

                    case ExecuteSkillResult.InsufficientTime:
                        {
                            set.AddStat(SimulationStat.DelayTime, TestInterval);
                            break;
                        }

                    case ExecuteSkillResult.Success:
                        {
                            // We are done
                            continue;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                // The skill was not executed so pass some time
                data.CurrentTime += TestInterval;
            }

            return set;
        }
    }
}
