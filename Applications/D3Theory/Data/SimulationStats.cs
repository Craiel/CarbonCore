namespace D3Theory.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;

    using CarbonCore.Utils;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class SimulationStats
    {
        public SimulationStats()
        {
            this.EntityAttributes = new Dictionary<D3EntityAttribute, float>();
            this.Stats = new Dictionary<SimulationStat, float>();

            this.SampleSets = new List<SimulationSampleSet>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DefaultValue(null)]
        public string Class { get; set; }

        [DefaultValue(0)]
        public int Seconds { get; set; }

        [DefaultValue(0)]
        public int Killed { get; set; }

        public Dictionary<D3EntityAttribute, float> EntityAttributes { get; set; }

        public Dictionary<SimulationStat, float> Stats { get; set; }

        public List<SimulationSampleSet> SampleSets { get; set; }

        [DefaultValue(null)]
        public Dictionary<D3Attribute, float> MergedAttributes { get; set; }

        public void AddStat(SimulationStat stat, float value = 1.0f)
        {
            if (!this.Stats.ContainsKey(stat))
            {
                this.Stats.Add(stat, value);
                return;
            }

            this.Stats[stat] += value;
        }

        public float GetTotalDamage(D3DamageType? type = null)
        {
            float total = 0.0f;
            foreach (SimulationSampleSet sampleSet in this.SampleSets)
            {
                foreach (D3DamageType damageType in sampleSet.DamageTotal.Keys)
                {
                    if (type != null && damageType != type.Value)
                    {
                        continue;
                    }

                    total += sampleSet.DamageTotal[damageType];
                }
            }

            return total;
        }

        public string ExportAsText()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("Simulation for Class {0} over {1} seconds\n", this.Class, this.Seconds);
            builder.AppendLine();

            if (this.EntityAttributes != null && this.EntityAttributes.Count > 0)
            {
                builder.AppendFormat("Attributes: \n");
                foreach (D3EntityAttribute key in this.EntityAttributes.Keys.OrderBy(x => x))
                {
                    builder.AppendCommaSeparatedLine(string.Empty, key, this.EntityAttributes[key]);
                }

                builder.AppendLine();
            }

            if (this.Stats != null && this.Stats.Count > 0)
            {
                builder.AppendFormat("Stats: \n");
                foreach (SimulationStat key in this.Stats.Keys.OrderBy(x => x))
                {
                    builder.AppendCommaSeparatedLine(string.Empty, key, this.Stats[key]);
                }

                builder.AppendLine();
            }

            if (this.MergedAttributes != null && this.MergedAttributes.Count > 0)
            {
                builder.AppendFormat("MergedAttributes: \n");
                foreach (D3Attribute key in this.MergedAttributes.Keys.OrderBy(x => x))
                {
                    builder.AppendCommaSeparatedLine(string.Empty, key, this.MergedAttributes[key]);
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        public string ExportSetsAsTable()
        {
            var builder = new StringBuilder();

            builder.AppendCommaSeparated("Skill", "DamageType", "DPS");
            builder.Append(",");
            IList<string> columnNames = SimulationSampleSet.GetDamageColumnNames();
            for (int i = 0; i < columnNames.Count; i++)
            {
                builder.Append(columnNames[i]);
                if (i < columnNames.Count - 1)
                {
                    builder.Append(',');
                }
            }

            builder.Append(Environment.NewLine);

            foreach (SimulationSampleSet set in this.SampleSets)
            {
                foreach (D3DamageType damageType in set.DamageTypes)
                {
                    string dpsString = this.FormatFloatForOutput(set.GetDPS(damageType, this.Seconds));
                    builder.AppendCommaSeparated(set.Name, damageType, dpsString);
                    builder.Append(',');
                    IList<float> values = set.GetDamageValues(damageType);
                    for (int i = 0; i < values.Count; i++)
                    {
                        builder.Append(this.FormatFloatForOutput(values[i]));
                        if (i < values.Count - 1)
                        {
                            builder.Append(',');
                        }
                    }

                    builder.Append(Environment.NewLine);
                }
            }

            return builder.ToString();
        }

        private string FormatFloatForOutput(float value)
        {
            double rounded = Math.Round(value);
            return rounded.ToString("0");
        }
    }
}
