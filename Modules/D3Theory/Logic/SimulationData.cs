namespace CarbonCore.Modules.D3Theory.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.Utils.Diagnostics;

    public class SimulationData : ISimulationData
    {
        private readonly Random random;

        private readonly List<IEntity> targets;

        private readonly IDictionary<SkillCombo, float> cooldownSkills;
        private readonly IDictionary<SkillCombo, float> runningSkills;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SimulationData(IMainData mainData, Simulation simulation, bool randomValues = false)
        {
            this.random = randomValues ? new Random((int)DateTime.Now.Ticks) : new Random(int.MaxValue);
            this.targets = new List<IEntity>();
            this.cooldownSkills = new Dictionary<SkillCombo, float>();
            this.runningSkills = new Dictionary<SkillCombo, float>();

            this.MainData = mainData;
            this.Simulation = simulation;

            this.Class = this.MainData.Classes.FirstOrDefault(x => x.Name.Equals(this.Simulation.Class, StringComparison.OrdinalIgnoreCase));

            if (!this.Validate())
            {
                return;
            }
            
            this.AttributeSet = new AttributeSet(this.Class);
            this.AttributeSetTemp = new AttributeSet(this.Class, false);
            this.AttributeSet.Merge(simulation.Attributes);
            if (simulation.Gear != null)
            {
                foreach (D3Gear gear in simulation.Gear)
                {
                    if (!gear.Enabled)
                    {
                        continue;
                    }

                    this.AttributeSet.Merge(gear.Attributes);
                }
            }

            this.Stats = new SimulationStats
                             {
                                 Class = simulation.Class,
                                 MergedAttributes = this.AttributeSet.GetAttributes(),
                                 Seconds = simulation.Seconds
                             };

            this.Character = new Entity(this.AttributeSet);

            this.MaxTime = simulation.Seconds;

            this.IsValid = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsValid { get; private set; }

        public IMainData MainData { get; private set; }
        public Simulation Simulation { get; private set; }
        public AttributeSet AttributeSet { get; private set; }
        public AttributeSet AttributeSetTemp { get; private set; }
        public SimulationStats Stats { get; private set; }
        public IEntity Character { get; private set; }

        public IReadOnlyCollection<IEntity> Targets
        {
            get
            {
                return this.targets.AsReadOnly();
            }
        }

        public D3Class Class { get; private set; }
        public SkillCombo SelectedSkill { get; set; }

        public float CurrentTime { get; set; }
        public float MaxTime { get; set; }

        public float Random(float min = 0.0f, float max = 1.0f)
        {
            return min + ((float)this.random.NextDouble() * (max - min));
        }
        
        public bool SelectSkill(string skillName, string runeName)
        {
            D3Skill skill = this.Class.GetSkill(skillName);
            if (skill == null)
            {
                return false;
            }

            D3SkillRune rune = null;
            if (!string.IsNullOrEmpty(runeName))
            {
                rune = skill.GetRune(runeName);
                if (rune == null)
                {
                    return false;
                }
            }

            this.SelectedSkill = new SkillCombo(skill, rune);
            return true;
        }
        
        public void ClearTargets()
        {
            this.targets.Clear();
        }

        public void UpdateTargets(SimulationSampleSet sampleSet)
        {
            IList<IEntity> entities = new List<IEntity>(this.targets);
            foreach (IEntity entity in entities)
            {
                if (!entity.IsAlive)
                {
                    // Todo: - log and award
                    this.targets.Remove(entity);
                    sampleSet.AddStat(SimulationStat.TargetsKilled);
                }
            }

            if (this.targets.Count > 0)
            {
                return;
            }

            int newCount = this.random.Next(this.Simulation.TargetCountMin, this.Simulation.TargetCountMax);
            for (int i = 0; i < newCount; i++)
            {
                IEntity target = new Entity(this.AttributeSet);
                this.targets.Add(target);
            }
        }

        public IEntity PickRandomTarget()
        {
            if (this.targets.Count == 0)
            {
                return null;
            }

            return this.targets[this.random.Next(0, this.targets.Count - 1)];
        }

        public IList<IEntity> PickRandomTargets(int max)
        {
            if (this.targets.Count <= 0 || max <= 0)
            {
                return null;
            }
            
            int count = this.random.Next(1, max);
            IList<IEntity> candidates = new List<IEntity>(this.targets);
            IList<IEntity> results = new List<IEntity>();
            while (results.Count < count && candidates.Count > 0)
            {
                IEntity target = candidates[this.random.Next(0, candidates.Count - 1)];
                results.Add(target);
                candidates.Remove(target);
            }

            return results;
        }

        public void SetSkillRunning(SkillCombo skill, float timeUntil)
        {
            if (!this.runningSkills.ContainsKey(skill))
            {
                this.runningSkills.Add(skill, timeUntil);
                return;
            }

            // We already have this so update it's time
            this.runningSkills[skill] = timeUntil;
        }

        public IList<SkillCombo> GetRunningSkills()
        {
            return new List<SkillCombo>(this.runningSkills.Keys);
        }

        public bool IsRunning(SkillCombo skill)
        {
            if (!this.runningSkills.ContainsKey(skill))
            {
                return false;
            }

            return this.runningSkills[skill] > this.CurrentTime;
        }

        public void SetSkillCooldown(SkillCombo skill, float timeUntil)
        {
            if (!this.cooldownSkills.ContainsKey(skill))
            {
                this.cooldownSkills.Add(skill, timeUntil);
                return;
            }

            // We already have this so update it's time
            this.cooldownSkills[skill] = timeUntil;
        }

        public IList<SkillCombo> GetCooldownSkills()
        {
            return new List<SkillCombo>(this.cooldownSkills.Keys);
        }

        public bool IsOnCooldown(SkillCombo skill)
        {
            if (!this.cooldownSkills.ContainsKey(skill))
            {
                return false;
            }

            return this.cooldownSkills[skill] > this.CurrentTime;
        }

        public void UpdateSkills()
        {
            IList<SkillCombo> skills = this.GetRunningSkills();
            foreach (SkillCombo skill in skills)
            {
                if (!this.IsRunning(skill))
                {
                    this.runningSkills.Remove(skill);
                }
            }

            skills = this.GetCooldownSkills();
            foreach (SkillCombo skill in skills)
            {
                if (!this.IsOnCooldown(skill))
                {
                    this.cooldownSkills.Remove(skill);
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool Validate()
        {
            if (this.Class == null)
            {
                Diagnostic.Error("Class not found: {0}", this.Simulation.Class);
                return false;
            }

            return true;
        }
    }
}
