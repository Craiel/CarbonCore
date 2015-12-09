namespace CarbonCore.Modules.D3Theory.Logic
{
    using System.Collections.Generic;

    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;

    public class Entity : IEntity
    {
        private readonly IDictionary<D3EntityAttribute, float> attributes;

        private readonly IAttributeSet attributeSet;

        private bool initialized;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Entity(IAttributeSet attributeSet)
        {
            this.attributeSet = attributeSet;
            this.attributeSet.AttributesChanged += this.OnAttributesChanged;
            
            this.attributes = new Dictionary<D3EntityAttribute, float>();

            // Call once to initialize
            this.OnAttributesChanged();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsAlive
        {
            get
            {
                return this.GetValue(D3EntityAttribute.Life) > 0.0f;
            }
        }

        public void Dispose()
        {
            this.attributeSet.AttributesChanged -= this.OnAttributesChanged;
        }

        public float GetValue(D3EntityAttribute attribute)
        {
            if (!this.attributes.ContainsKey(attribute))
            {
                return 0.0f;
            }

            return this.attributes[attribute];
        }

        public void SetValue(D3EntityAttribute attribute, float value)
        {
            if (!this.attributes.ContainsKey(attribute))
            {
                this.attributes.Add(attribute, value);
                return;
            }

            this.attributes[attribute] = value;
        }

        public float AddValue(D3EntityAttribute attribute, float value, float max)
        {
            if (value > max)
            {
                value = max;
            }

            if (!this.attributes.ContainsKey(attribute))
            {
                this.attributes.Add(attribute, value);
                return value;
            }

            if (this.attributes[attribute] + value > max)
            {
                value -= (this.attributes[attribute] + value) - max;
            }

            this.attributes[attribute] += value;
            return value;
        }

        public float RemoveValue(D3EntityAttribute attribute, float value)
        {
            if (value <= 0.0f || !this.attributes.ContainsKey(attribute))
            {
                return 0.0f;
            }

            if (value > this.attributes[attribute])
            {
                value = this.attributes[attribute];
            }

            this.attributes[attribute] -= value;
            return value;
        }

        // This will make sure we are always equal or below the limits of entity stats that require this
        private void OnAttributesChanged()
        {
            IDictionary<D3Attribute, float> coreAttributes = this.attributeSet.GetAttributes();
            foreach (D3Attribute attribute in coreAttributes.Keys)
            {
                float value = coreAttributes[attribute];
                switch (attribute)
                {
                    case D3Attribute.Life:
                        {
                            if (!this.initialized || this.GetValue(D3EntityAttribute.Life) > value)
                            {
                                this.SetValue(D3EntityAttribute.Life, value);
                            }

                            break;
                        }

                    case D3Attribute.PrimaryResource:
                        {
                            if (!this.initialized || this.GetValue(D3EntityAttribute.PrimaryResource) > value)
                            {
                                this.SetValue(D3EntityAttribute.PrimaryResource, value);
                            }

                            break;
                        }

                    case D3Attribute.SecondaryResource:
                        {
                            if (!this.initialized || this.GetValue(D3EntityAttribute.SecondaryResource) > value)
                            {
                                this.SetValue(D3EntityAttribute.SecondaryResource, value);
                            }

                            break;
                        }
                }
            }

            this.initialized = true;
        }
    }
}
