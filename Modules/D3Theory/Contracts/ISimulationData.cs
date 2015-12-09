namespace CarbonCore.Modules.D3Theory.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.Modules.D3Theory.Logic;

    public interface ISimulationData
    {
        IMainData MainData { get; }
        Simulation Simulation { get; }
        AttributeSet AttributeSet { get; }
        SimulationStats Stats { get; }
        IEntity Character { get; }

        D3Class Class { get; }
        SkillCombo SelectedSkill { get; set; }

        IReadOnlyCollection<IEntity> Targets { get; }

        float CurrentTime { get; set; }
        float MaxTime { get; set; }

        float Random(float min = 0.0f, float max = 1.0f);

        bool SelectSkill(string skill, string rune);

        void ClearTargets();
        void UpdateTargets(SimulationSampleSet sampleSet);

        IEntity PickRandomTarget();
        IList<IEntity> PickRandomTargets(int max);

        void SetSkillRunning(SkillCombo skill, float timeUntil);
        IList<SkillCombo> GetRunningSkills();
        bool IsRunning(SkillCombo skill);

        void SetSkillCooldown(SkillCombo skill, float timeUntil);
        IList<SkillCombo> GetCooldownSkills();
        bool IsOnCooldown(SkillCombo skill);

        void UpdateSkills();
    }
}
