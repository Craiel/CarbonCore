namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class ContentEntryDescriptor
    {
        public static readonly IDictionary<Type, ContentEntryDescriptor> Descriptors = new Dictionary<Type, ContentEntryDescriptor>();

        private readonly List<PropertyInfo> clonableProperties;
        private readonly List<PropertyInfo> equalityProperties;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ContentEntryDescriptor(Type targetType)
        {
            System.Diagnostics.Trace.Assert(typeof(IContentEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.clonableProperties = new List<PropertyInfo>();
            this.equalityProperties = new List<PropertyInfo>();

            this.PropertyInfos = new List<AttributedPropertyInfo<ContentEntryElementAttribute>>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }
        
        public IList<AttributedPropertyInfo<ContentEntryElementAttribute>> PropertyInfos { get; private set; }

        public IReadOnlyCollection<PropertyInfo> ClonableProperties
        {
            get
            {
                return this.clonableProperties.AsReadOnly();
            }
        }

        public IReadOnlyCollection<PropertyInfo> EqualityProperties
        {
            get
            {
                return this.equalityProperties.AsReadOnly();
            }
        }

        public static ContentEntryDescriptor GetDescriptor<T>()
        {
            return GetDescriptor(typeof(T));
        }

        public static ContentEntryDescriptor GetDescriptor(Type type)
        {
            if (!Descriptors.ContainsKey(type))
            {
                Descriptors.Add(type, new ContentEntryDescriptor(type));
            }

            return Descriptors[type];
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as ContentEntryDescriptor;
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
            this.PropertyInfos.Clear();

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                bool ignoreClone = false;
                bool ignoreEquality = false;
                foreach (object attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    AttributedPropertyInfo<ContentEntryElementAttribute> element = null;

                    if (attributeType == typeof(ContentEntryElementAttribute))
                    {
                        element = new AttributedPropertyInfo<ContentEntryElementAttribute>(this.Type, (ContentEntryElementAttribute)attribute, info);
                    }

                    if (element != null)
                    {
                        ignoreClone = element.Attribute.IgnoreClone;
                        ignoreEquality = element.Attribute.IgnoreEquality;

                        this.PropertyInfos.Add(element);
                    }
                }

                if (!ignoreClone)
                {
                    this.clonableProperties.Add(info);
                }

                if (!ignoreEquality)
                {
                    this.equalityProperties.Add(info);
                }
            }
        }
    }
}
