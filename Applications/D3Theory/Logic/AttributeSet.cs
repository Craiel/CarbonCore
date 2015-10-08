namespace D3Theory.Logic
{
    using System;
    using System.Collections.Generic;

    using D3Theory.Contracts;
    using D3Theory.Data;

    public class AttributeSet : IAttributeSet
    {
        private readonly IDictionary<D3Attribute, float> attributes;
        private readonly IDictionary<D3Attribute, float> finalAttributes;

        private readonly D3Class currentClass;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AttributeSet(D3Class @class, bool initializeWithClass = true)
        {
            this.currentClass = @class;
            this.attributes = initializeWithClass ? new Dictionary<D3Attribute, float>(@class.Attributes) : new Dictionary<D3Attribute, float>();

            this.finalAttributes = new Dictionary<D3Attribute, float>();

            this.DamageType = D3DamageType.Physical;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action AttributesChanged;

        public D3DamageType DamageType { get; set; }

        public void Merge(IDictionary<D3Attribute, float> other, float factor = 1.0f)
        {
            foreach (KeyValuePair<D3Attribute, float> pair in other)
            {
                if (!this.attributes.ContainsKey(pair.Key))
                {
                    this.attributes.Add(pair.Key, 0);
                }

                this.MergeAttribute(pair.Key, pair.Value * factor);
            }

            this.RecalculateAttributes();
        }

        public void Merge(IAttributeSet other, float factor = 1.0f)
        {
            this.Merge(other.GetAttributes(), factor);
        }

        public void Clear()
        {
            this.attributes.Clear();
            this.finalAttributes.Clear();
        }

        public Dictionary<D3Attribute, float> GetAttributes()
        {
            return new Dictionary<D3Attribute, float>(this.finalAttributes);
        }

        public float GetValue(D3Attribute attribute)
        {
            if (this.finalAttributes.ContainsKey(attribute))
            {
                return this.finalAttributes[attribute];
            }

            return 0.0f;
        }

        public void SetValue(D3Attribute attribute, float value)
        {
            if (!this.attributes.ContainsKey(attribute))
            {
                this.attributes.Add(attribute, value);
            }

            this.attributes[attribute] = value;

            this.RecalculateAttributes();
        }

        public void AddValue(D3Attribute attribute, float value, float maxValue)
        {
            if (!this.attributes.ContainsKey(attribute))
            {
                this.attributes.Add(attribute, value);
                return;
            }

            this.attributes[attribute] += value;
            if (this.attributes[attribute] > maxValue)
            {
                this.attributes[attribute] = maxValue;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void MergeAttribute(D3Attribute attribute, float value)
        {
            switch (attribute)
            {
                // These replace instead of add
                case D3Attribute.AttackSpeed:
                    {
                        this.attributes[attribute] = value;
                        break;
                    }

                default:
                    {
                        this.attributes[attribute] += value;
                        break;
                    }
            }
        }

        private bool AddPrimaryAttributeFinalValue(D3Attribute attribute, float value)
        {
            switch (attribute)
            {
                case D3Attribute.Str:
                case D3Attribute.GemStr:
                    {
                        if (this.currentClass.PrimaryAttribute == D3Attribute.Str)
                        {
                            this.AddFinalAttribute(D3Attribute.DmgIncreasePrimary, value);
                        }

                        this.AddFinalAttribute(D3Attribute.Armor, value);
                        return true;
                    }

                case D3Attribute.Dex:
                case D3Attribute.GemDex:
                    {
                        if (this.currentClass.PrimaryAttribute == D3Attribute.Dex)
                        {
                            this.AddFinalAttribute(D3Attribute.DmgIncreasePrimary, value);
                        }

                        this.AddFinalAttribute(D3Attribute.DodgeRate, value / 216.0f);
                        return true;
                    }

                case D3Attribute.Int:
                case D3Attribute.GemInt:
                    {
                        if (this.currentClass.PrimaryAttribute == D3Attribute.Int)
                        {
                            this.AddFinalAttribute(D3Attribute.DmgIncreasePrimary, value);
                        }

                        this.AddFinalResistAll(value / 10.0f);
                        return true;
                    }
            }

            return false;
        }

        private void RecalculateAttributes()
        {
            this.finalAttributes.Clear();

            IDictionary<D3Attribute, float> multipliers = new Dictionary<D3Attribute, float>();
            
            foreach (D3Attribute attribute in this.attributes.Keys)
            {
                float value = this.attributes[attribute];

                if (this.AddPrimaryAttributeFinalValue(attribute, value))
                {
                    continue;
                }

                switch (attribute)
                {
                    // Mute out attributes we don't want to see
                    case D3Attribute.LifePerVit:
                    case D3Attribute.Indestructable:
                        {
                            break;
                        }

                    case D3Attribute.ResistAll:
                        {
                            this.AddFinalResistAll(value);
                            break;
                        }

                    case D3Attribute.Vit:
                    case D3Attribute.GemVit:
                        {
                            this.AddFinalAttribute(D3Attribute.Life, value * this.currentClass.Attributes[D3Attribute.LifePerVit]);
                            break;
                        }

                    case D3Attribute.DmgBonus:
                        {
                            multipliers.Add(D3Attribute.DmgMin, value);
                            multipliers.Add(D3Attribute.DmgMax, value);
                            break;
                        }

                    case D3Attribute.LifeBonus:
                        {
                            multipliers.Add(D3Attribute.Life, value);
                            break;
                        }

                    case D3Attribute.AttackSpeedBonus:
                        {
                            // Todo: we might have to apply this after dual wield calculation...
                            multipliers.Add(D3Attribute.AttackSpeed, value);
                            multipliers.Add(D3Attribute.AttackSpeedOffhand, value);
                            break;
                        }

                    default:
                        {
                            // Default we just move over
                            this.AddFinalAttribute(attribute, value);
                            break;
                        }
                }
            }

            // Apply multipliers
            foreach (D3Attribute attribute in multipliers.Keys)
            {
                this.MultiplyFinalAttribute(attribute, multipliers[attribute]);
            }

            if (this.AttributesChanged != null)
            {
                this.AttributesChanged();
            }
        }

        private void MultiplyFinalAttribute(D3Attribute attribute, float value)
        {
            if (!this.finalAttributes.ContainsKey(attribute))
            {
                return;
            }

            float current = this.finalAttributes[attribute];
            this.finalAttributes[attribute] += current  * (value / 100);
        }

        private void AddFinalAttribute(D3Attribute attribute, float value)
        {
            if (!this.finalAttributes.ContainsKey(attribute))
            {
                this.finalAttributes.Add(attribute, value);
                return;
            }

            this.finalAttributes[attribute] += value;
        }

        private void AddFinalResistAll(float value)
        {
            this.AddFinalAttribute(D3Attribute.ResistArcane, value);
            this.AddFinalAttribute(D3Attribute.ResistCold, value);
            this.AddFinalAttribute(D3Attribute.ResistFire, value);
            this.AddFinalAttribute(D3Attribute.ResistLightning, value);
            this.AddFinalAttribute(D3Attribute.ResistPhysical, value);
            this.AddFinalAttribute(D3Attribute.ResistPoison, value);
        }
    }
}
