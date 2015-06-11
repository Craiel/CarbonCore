namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data;
    using System.Reflection;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Database;

    public class DatabaseEntryElementDescriptor : AttributedPropertyInfo<DatabaseEntryElementAttribute>
    {
        public DatabaseEntryElementDescriptor(Type host, DatabaseEntryElementAttribute attribute, PropertyInfo property)
            : base(host, attribute, property)
        {
        }

        public string Name
        {
            get
            {
                return this.Attribute.Name ?? this.Property.Name;
            }
        }

        public Type InternalType
        {
            get
            {
                return this.Property.PropertyType;
            }
        }

        public SqlDbType DatabaseType
        {
            get
            {
                return DatabaseUtils.GetDatabaseType(this.Property.PropertyType);
            }
        }
    }
}
