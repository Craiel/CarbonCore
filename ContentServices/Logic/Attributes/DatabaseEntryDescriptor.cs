namespace CarbonCore.ContentServices.Logic.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils;

    public class DatabaseEntryDescriptor
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntryDescriptor(Type targetType)
        {
            System.Diagnostics.Trace.Assert(typeof(IDatabaseEntry).IsAssignableFrom(targetType));

            this.Type = targetType;

            this.PropertyInfos = new List<AttributedPropertyInfo<DatabaseEntryElementAttribute>>();

            this.Analyze();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Type { get; private set; }

        public DatabaseEntryAttribute EntryInfo { get; private set; }

        public AttributedPropertyInfo<DatabaseEntryPrimaryKeyAttribute> PropertyPrimaryKey { get; private set; }

        public IList<AttributedPropertyInfo<DatabaseEntryElementAttribute>> PropertyInfos { get; private set; }

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

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Analyze()
        {
            this.EntryInfo = this.Type.GetCustomAttribute<DatabaseEntryAttribute>();
            this.PropertyPrimaryKey = null;
            this.PropertyInfos.Clear();

            PropertyInfo[] properties = this.Type.GetProperties();
            foreach (PropertyInfo info in properties)
            {
                object[] attributes = info.GetCustomAttributes(true);
                foreach (object attribute in attributes)
                {
                    Type attributeType = attribute.GetType();
                    if (attributeType == typeof(DatabaseEntryPrimaryKeyAttribute))
                    {
                        this.PropertyPrimaryKey = new AttributedPropertyInfo<DatabaseEntryPrimaryKeyAttribute>(
                            this.Type, (DatabaseEntryPrimaryKeyAttribute)attribute, info);

                        continue;
                    }

                    if (attributeType == typeof(DatabaseEntryElementAttribute))
                    {
                        this.PropertyInfos.Add(
                            new AttributedPropertyInfo<DatabaseEntryElementAttribute>(
                                this.Type, (DatabaseEntryElementAttribute)attribute, info));

                        continue;
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
