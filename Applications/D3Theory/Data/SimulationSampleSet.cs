namespace D3Theory.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class SimulationSampleSet
    {
        private readonly List<D3DamageType> damageTypes;
 
        public SimulationSampleSet()
        {
            this.damageTypes = new List<D3DamageType>();

            this.Stats = new Dictionary<SimulationStat, float>();

            this.DamageTotal = new Dictionary<D3DamageType, float>();
            this.DamageMin = new Dictionary<D3DamageType, float>();
            this.DamageMax = new Dictionary<D3DamageType, float>();
            this.DamageAverage = new Dictionary<D3DamageType, float>();
            this.DamageCount = new Dictionary<D3DamageType, float>();
            this.DamageNormal = new Dictionary<D3DamageType, float>();
            this.DamageCrit = new Dictionary<D3DamageType, float>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DefaultValue(null)]
        public string Name { get; set; }

        [DefaultValue(null)]
        public string Skill { get; set; }

        [DefaultValue(null)]
        public string Rune { get; set; }

        [JsonIgnore]
        public IReadOnlyCollection<D3DamageType> DamageTypes
        {
            get
            {
                return this.damageTypes.AsReadOnly();
            }
        }

        public Dictionary<SimulationStat, float> Stats { get; set; }

        public Dictionary<D3DamageType, float> DamageTotal { get; set; }
        public Dictionary<D3DamageType, float> DamageMin { get; set; }
        public Dictionary<D3DamageType, float> DamageMax { get; set; }
        public Dictionary<D3DamageType, float> DamageAverage { get; set; }
        public Dictionary<D3DamageType, float> DamageCount { get; set; }
        public Dictionary<D3DamageType, float> DamageNormal { get; set; }
        public Dictionary<D3DamageType, float> DamageCrit { get; set; }

        // Has to match GetDamageValues()
        public static IList<string> GetDamageColumnNames()
        {
            return new List<string> { "Total", "Min", "Max", "Average", "Count", "Normal", "Critical" };
        }

        public void AddDamage(D3DamageType type, float value, bool isCritical)
        {
            if (!this.damageTypes.Contains(type))
            {
                this.damageTypes.Add(type);
            }

            if (isCritical)
            {
                this.AddStat(SimulationStat.Crit);
            }

            this.InitializeDamageType(type);

            this.DamageTotal[type] += value;
            if (this.DamageMin[type] > value)
            {
                this.DamageMin[type] = value;
            }

            if (this.DamageMax[type] < value)
            {
                this.DamageMax[type] = value;
            }

            this.DamageCount[type]++;

            this.DamageAverage[type] = this.DamageTotal[type] / this.DamageCount[type];

            if (isCritical)
            {
                this.DamageCrit[type] += value;
            }
            else
            {
                this.DamageNormal[type] += value;
            }
        }

        public void UpdateDPS(float secondsElapsed)
        {
            this.AddStat(SimulationStat.Dps, 0.0f);

            float total = 0.0f;
            foreach (D3DamageType type in this.DamageTotal.Keys)
            {
                total += this.DamageTotal[type];
            }

            this.AddStat(SimulationStat.Dps, total / secondsElapsed);
        }

        public void AddStat(SimulationStat stat, float value = 1.0f)
        {
            if (Math.Abs(value - 0.0f) < float.Epsilon)
            {
                return;
            }

            if (!this.Stats.ContainsKey(stat))
            {
                this.Stats.Add(stat, value);
                return;
            }

            this.Stats[stat] += value;
        }

        // Used in table writing
        public IList<float> GetDamageValues(D3DamageType damageType)
        {
            IList<float> results = new List<float>();
            results.Add(this.DamageTotal.ContainsKey(damageType) ? this.DamageTotal[damageType] : 0f);
            results.Add(this.DamageMin.ContainsKey(damageType) ? this.DamageMin[damageType] : 0f);
            results.Add(this.DamageMax.ContainsKey(damageType) ? this.DamageMax[damageType] : 0f);
            results.Add(this.DamageAverage.ContainsKey(damageType) ? this.DamageAverage[damageType] : 0f);
            results.Add(this.DamageCount.ContainsKey(damageType) ? this.DamageCount[damageType] : 0f);
            results.Add(this.DamageNormal.ContainsKey(damageType) ? this.DamageNormal[damageType] : 0f);
            results.Add(this.DamageCrit.ContainsKey(damageType) ? this.DamageCrit[damageType] : 0f);
            return results;
        }

        public float GetDPS(D3DamageType damageType, int seconds)
        {
            if (seconds <= 0)
            {
                return 0f;
            }

            float total = this.DamageTotal.ContainsKey(damageType) ? this.DamageTotal[damageType] : 0f;
            return total / (float)seconds;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void InitializeDamageType(D3DamageType type)
        {
            // Only need to check one
            if (this.DamageTotal.ContainsKey(type))
            {
                return;
            }

            this.DamageTotal.Add(type, 0);
            this.DamageMin.Add(type, float.MaxValue);
            this.DamageMax.Add(type, 0);
            this.DamageAverage.Add(type, 0);
            this.DamageCount.Add(type, 0);
            this.DamageNormal.Add(type, 0);
            this.DamageCrit.Add(type, 0);
        }
    }
}
