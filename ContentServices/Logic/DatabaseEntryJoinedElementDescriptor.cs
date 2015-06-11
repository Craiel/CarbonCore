namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data;
    using System.Reflection;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Database;

    public class DatabaseEntryJoinedElementDescriptor : AttributedPropertyInfo<DatabaseEntryJoinedElementAttribute>
    {
        public DatabaseEntryJoinedElementDescriptor(Type host, DatabaseEntryJoinedElementAttribute attribute, PropertyInfo property)
            : base(host, attribute, property)
        {
        }

        public string ForeignKeyColumn
        {
            get
            {
                return this.Attribute.ForeignKeyColumn;
            }
        }

        public PropertyInfo ForeignKeyProperty
        {
            get
            {
                return this.InternalType.GetProperty(this.Attribute.ForeignKeyColumn);
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
