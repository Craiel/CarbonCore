namespace D3Theory.Logic
{
    using D3Theory.Data;

    public class SkillCombo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SkillCombo(D3Skill skill, D3SkillRune rune)
        {
            this.Skill = skill;
            this.Rune = rune;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public D3Skill Skill { get; private set; }
        public D3SkillRune Rune { get; private set; }

        public float LastTrigger { get; set; }

        public float GetValue(D3SkillAttribute attribute)
        {
            float value = this.Skill.GetValue(attribute);
            if (this.Rune != null)
            {
                value += this.Rune.GetValue(attribute);
            }

            return value;
        }

        public D3DamageType GetDamageType()
        {
            D3DamageType type = this.Skill.DamageType;
            if (this.Rune != null)
            {
                type = this.Rune.DamageType;
            }

            return type;
        }

        public void MergeAttributes(AttributeSet target)
        {
            if (this.Skill.Attributes != null)
            {
                target.Merge(this.Skill.Attributes);
            }

            if (this.Rune != null && this.Rune.Attributes != null)
            {
                target.Merge(this.Rune.Attributes);
            }
        }
    }
}
