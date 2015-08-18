namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class DataEntryDescriptor
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DataEntryDescriptor(Type targetType)
        {
            System.Diagnostics.Trace.Assert(typeof(IDataEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.PropertyInfo = new List<AttributedPropertyInfo<DataElementAttribute>>();

            this.CloneableProperties = new List<AttributedPropertyInfo<DataElementAttribute>>();
            this.SerializableProperties = new List<AttributedPropertyInfo<DataElementAttribute>>();
            this.EqualityProperties = new List<AttributedPropertyInfo<DataElementAttribute>>();
            this.HashableProperties = new List<AttributedPropertyInfo<DataElementAttribute>>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public bool UseDefaultEquality { get; private set; }

        public bool OptOutAttributes { get; private set; }
        
        public IList<AttributedPropertyInfo<DataElementAttribute>> PropertyInfo { get; private set; }
        
        public IList<AttributedPropertyInfo<DataElementAttribute>> CloneableProperties { get; private set; }
        public IList<AttributedPropertyInfo<DataElementAttribute>> SerializableProperties { get; private set; }
        public IList<AttributedPropertyInfo<DataElementAttribute>> EqualityProperties { get; private set; }
        public IList<AttributedPropertyInfo<DataElementAttribute>> HashableProperties { get; private set; }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as DataEntryDescriptor;
            if (other == null)
            {
                return false;
            }

            return other.Type == this.Type;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            this.PropertyInfo.Clear();

            object[] entryAttributes = this.Type.GetCustomAttributes(true);
            foreach (object attribute in entryAttributes)
            {
                var dataAttribute = attribute as DataEntryAttribute;
                if (dataAttribute != null)
                {
                    this.UseDefaultEquality = dataAttribute.UseDefaultEquality;
                    this.OptOutAttributes = dataAttribute.OptOutAttributes;
                }
            }

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    DataElementAttribute typed = attribute as DataElementAttribute;
                    if (typed != null)
                    {
                        var elementInfo = new AttributedPropertyInfo<DataElementAttribute>(this.Type, typed, info);
                        this.PropertyInfo.Add(elementInfo);

                        if (!elementInfo.Attribute.IgnoreClone)
                        {
                            this.CloneableProperties.Add(elementInfo);
                        }

                        if (!elementInfo.Attribute.IgnoreEquality)
                        {
                            this.EqualityProperties.Add(elementInfo);
                        }

                        if (!elementInfo.Attribute.IgnoreGetHashCode)
                        {
                            this.HashableProperties.Add(elementInfo);
                        }

                        if (!elementInfo.Attribute.IgnoreSerialization)
                        {
                            this.SerializableProperties.Add(elementInfo);
                        }
                    }
                }
            }
        }
    }
}
