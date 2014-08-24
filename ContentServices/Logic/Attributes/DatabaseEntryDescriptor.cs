namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils;

    public class DatabaseEntryDescriptor
    {
        public static readonly IDictionary<Type, DatabaseEntryDescriptor> Descriptors = new Dictionary<Type, DatabaseEntryDescriptor>();

        private readonly IDictionary<string, AttributedPropertyInfo<DatabaseEntryElementAttribute>> elementNameLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryDescriptor(Type targetType)
        {
            System.Diagnostics.Trace.Assert(typeof(IDatabaseEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.PropertyInfos = new List<AttributedPropertyInfo<DatabaseEntryElementAttribute>>();
            this.elementNameLookup = new Dictionary<string, AttributedPropertyInfo<DatabaseEntryElementAttribute>>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public DatabaseEntryAttribute EntryAttribute { get; private set; }

        public AttributedPropertyInfo<DatabaseEntryPrimaryKeyAttribute> PropertyPrimaryKey { get; private set; }

        public IList<AttributedPropertyInfo<DatabaseEntryElementAttribute>> PropertyInfos { get; private set; }

        public static DatabaseEntryDescriptor GetDescriptor<T>()
        {
            return GetDescriptor(typeof(T));
        }

        public static DatabaseEntryDescriptor GetDescriptor(Type type)
        {
            if (!Descriptors.ContainsKey(type))
            {
                Descriptors.Add(type, new DatabaseEntryDescriptor(type));
            }

            return Descriptors[type];
        }

        public override int GetHashCode()
        {
            return this.Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as DatabaseEntryDescriptor;
            if (other == null)
            {
                return false;
            }

            return other.Type == this.Type;
        }

        public IList<string> GetElementNames()
        {
            IList<string> result = new List<string>();
            foreach (AttributedPropertyInfo<DatabaseEntryElementAttribute> info in this.PropertyInfos)
            {
                result.Add(info.Attribute.Name ?? info.Property.Name);
            }

            return result;
        }

        public AttributedPropertyInfo<DatabaseEntryElementAttribute> GetElementByName(string name)
        {
            if (this.elementNameLookup.ContainsKey(name))
            {
                return this.elementNameLookup[name];
            }

            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            this.PropertyPrimaryKey = null;
            this.PropertyInfos.Clear();

            this.EntryAttribute = this.Type.GetCustomAttribute<DatabaseEntryAttribute>();

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    AttributedPropertyInfo<DatabaseEntryElementAttribute> element = null;

                    if (attributeType == typeof(DatabaseEntryPrimaryKeyAttribute))
                    {
                        element = new AttributedPropertyInfo<DatabaseEntryElementAttribute>(this.Type, (DatabaseEntryElementAttribute)attribute, info);
                        this.PropertyPrimaryKey = new AttributedPropertyInfo<DatabaseEntryPrimaryKeyAttribute>(this.Type, (DatabaseEntryPrimaryKeyAttribute)attribute, info);
                    }
                    else if (attributeType == typeof(DatabaseEntryElementAttribute))
                    {
                        element = new AttributedPropertyInfo<DatabaseEntryElementAttribute>(this.Type, (DatabaseEntryElementAttribute)attribute, info);
                    }

                    if (element != null)
                    {
                        this.PropertyInfos.Add(element);
                        this.elementNameLookup.Add(element.Attribute.Name ?? element.Property.Name, element);
                    }
                }
            }

            if (this.PropertyPrimaryKey == null)
            {
                System.Diagnostics.Trace.TraceWarning("DatabaseEntry has no Primary key defined: {0}", this.Type);
            }
        }
    }
}
